using AutoMapper;
using Core.Utilities.Results;
using DTO.Movies;
using DTO.Musics;
using Entity;
using Microsoft.EntityFrameworkCore;
using Service.Abstracts;

namespace Service.Concretes;

public class RecommendationService : IRecommendationService
{
    private readonly IMovieService _movieService;
    private readonly IMusicService _musicService;
    private readonly ITmdbApiService _tmdbApiService;
    private readonly ISpotifyApiService _spotifyApiService;
    private readonly AppDbContext _context;
    private readonly ICategoryLinkingService _categoryLinkingService;
    private readonly IMapper _mapper;

    public RecommendationService(
        IMovieService movieService,
        IMusicService musicService,
        ITmdbApiService tmdbApiService,
        ISpotifyApiService spotifyApiService,
        AppDbContext context,
        IMapper mapper, ICategoryLinkingService categoryLinkingService)
    {
        _movieService = movieService;
        _musicService = musicService;
        _tmdbApiService = tmdbApiService;
        _spotifyApiService = spotifyApiService;
        _context = context;
        _mapper = mapper;
        _categoryLinkingService = categoryLinkingService;
    }

    public async Task<IDataResult<List<GetMusicDto>>> GetMusicForMovieAsync(string movieTitle)
    {
        //Check local database if movie exists
        var localMovie = await _movieService.GetMovieByTitleAsync(movieTitle.ToUpper());
        Movie? targetMovie;

        if (localMovie.Success && localMovie.Data != null)
        {
            // Movie exists in database - reuse the already-fetched data
            targetMovie = _mapper.Map<Movie>(localMovie.Data);
            
            if (targetMovie == null)
            {
                return new ErrorDataResult<List<GetMusicDto>>("Failed to map movie data.");
            }
        }
        else
        {
            //Fetch movie from Tmdb if not found in local database
            var tmdbMovieDto = await _tmdbApiService.FetchMovieFromTmdbAsync(movieTitle);
            if (!tmdbMovieDto.Success || tmdbMovieDto.Data == null)
            {
                return new ErrorDataResult<List<GetMusicDto>>("Movie not found in local database or TMDb.");
            }
            
            //Map the data to CreateMovieDto
            var tmdbMovieMap = _mapper.Map<CreateMovieDto>(tmdbMovieDto.Data);
            
            //Save the newly fetched movie to database
            var createResult = await _movieService.CreateMovieAsync(tmdbMovieMap);
            if (!createResult.Success)
            {
                return new ErrorDataResult<List<GetMusicDto>>("Failed to save movie to database.");
            }

            // Fetch the newly saved movie with its relationships (Categories and SuggestedMusicCategories)
            targetMovie = _mapper.Map<Movie>((await _movieService.GetMovieByTmdbIdAsync(tmdbMovieMap.TmdbId)).Data);
            
            if (targetMovie == null)
            {
                return new ErrorDataResult<List<GetMusicDto>>("Failed to retrieve saved movie.");
            }
        }
        
        var movieCategoriesResult = await _categoryLinkingService.GetMovieCategoriesAsync(targetMovie.Id);
        if (!movieCategoriesResult.Success || movieCategoriesResult.Data == null)
        {
            return new ErrorDataResult<List<GetMusicDto>>("Failed to retrieve movie categories.");
        }
        
        //Get linked music genres
        var linkedMusicCategories = movieCategoriesResult.Data
            .SelectMany(mc => mc.SuggestedMusicCategories)
            .Select(mc => mc.Name)
            .Distinct()
            .ToList();

        if (!linkedMusicCategories.Any())
        {
             // Fallback genre if the movie had no specific links
             linkedMusicCategories.Add("rock");
        }
        
        // Select randomly 3 genres from the linked music categories
        var random = new Random();
        var selectedGenres = linkedMusicCategories
            .OrderBy(x => random.Next())
            .Take(3)
            .ToList();
        
        var allFetchedMusic = new List<GetMusicDto>();

        // Get 5 tracks from EACH of the 3 selected genres (total: 15 tracks)
        foreach (var genre in selectedGenres)
        {
            Console.WriteLine($"Fetching 5 tracks for genre: {genre}");
            
            var spotifyMusicDtos = await _spotifyApiService.FetchMusicByGenreAsync(genre, limit: 5);
            
            if (!spotifyMusicDtos.Success || spotifyMusicDtos.Data == null)
            {
                Console.WriteLine($"Failed to fetch music for genre '{genre}', skipping...");
                continue; // Skip this genre if failed
            }

            foreach (var newMusicDto in spotifyMusicDtos.Data)
            {
                // Check if we already have this specific Spotify ID in our DB
                var existingMusic = await _context.Set<Music>()
                    .FirstOrDefaultAsync(m => m.SpotifyId == newMusicDto.SpotifyId);
                    
                if (existingMusic == null)
                {
                    // Save new track using your existing CRUD service
                    await _musicService.CreateMusicAsync(newMusicDto);
                    allFetchedMusic.Add(_mapper.Map<GetMusicDto>(newMusicDto));
                }
                else
                {
                    // We already had it, just add it to the list
                    allFetchedMusic.Add(_mapper.Map<GetMusicDto>(existingMusic));
                }
            }
        }

        if (!allFetchedMusic.Any())
        {
            return new ErrorDataResult<List<GetMusicDto>>("No music tracks found for this movie.");
        }

        // Shuffle the 15 fetched tracks and take only 5 for the final list
        var shuffledList = allFetchedMusic.OrderBy(x => random.Next()).Take(5).ToList();
        
        return new SuccessDataResult<List<GetMusicDto>>(
            shuffledList, 
            $"Successfully generated {shuffledList.Count} music tracks from {selectedGenres.Count} randomly selected genres for {targetMovie.Title}");
    }
}