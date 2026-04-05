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
        // ==========================================
        // 1. CHECK LOCAL DATABASE FIRST
        // ==========================================
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
            // ==========================================
            // 2. CACHE MISS: FETCH FROM TMDB AND SAVE
            // ==========================================
            var tmdbMovieDto = await _tmdbApiService.FetchMovieFromTmdbAsync(movieTitle);
            if (!tmdbMovieDto.Success || tmdbMovieDto.Data == null)
            {
                return new ErrorDataResult<List<GetMusicDto>>("Movie not found in local database or TMDb.");
            }

            var tmdbMovieMap = _mapper.Map<CreateMovieDto>(tmdbMovieDto.Data);
            // Use your existing CRUD service to save it!
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

        // ==========================================
        // 3. GET LINKED MUSIC CATEGORIES
        // ==========================================
        var movieCategoriesResult = await _categoryLinkingService.GetMovieCategoriesAsync(targetMovie.Id);
        if (!movieCategoriesResult.Success || movieCategoriesResult.Data == null)
        {
            return new ErrorDataResult<List<GetMusicDto>>("Failed to retrieve movie categories.");
        }

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

        // ==========================================
        // 4. GET MUSIC FROM ALL LINKED GENRES
        // ==========================================
        
        var finalMusicList = new List<GetMusicDto>();

        // Get 5 tracks from EACH linked music genre
        foreach (var genre in linkedMusicCategories)
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
                    finalMusicList.Add(_mapper.Map<GetMusicDto>(newMusicDto));
                }
                else
                {
                    // We already had it, just add it to the return list
                    finalMusicList.Add(_mapper.Map<GetMusicDto>(existingMusic));
                }
            }
        }

        if (!finalMusicList.Any())
        {
            return new ErrorDataResult<List<GetMusicDto>>("No music tracks found for this movie.");
        }

        // Shuffle the music list for variety and take only 5
        var random = new Random();
        var shuffledList = finalMusicList.OrderBy(x => random.Next()).Take(5).ToList();

        // ==========================================
        // 5. RETURN SUCCESS
        // ==========================================
        return new SuccessDataResult<List<GetMusicDto>>(
            shuffledList, 
            $"Successfully generated {shuffledList.Count} music tracks from {linkedMusicCategories.Count} genres for {targetMovie.Title}");
    }
}