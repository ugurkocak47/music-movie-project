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
    private readonly IMapper _mapper;

    public RecommendationService(
        IMovieService movieService,
        IMusicService musicService,
        ITmdbApiService tmdbApiService,
        ISpotifyApiService spotifyApiService,
        AppDbContext context,
        IMapper mapper)
    {
        _movieService = movieService;
        _musicService = musicService;
        _tmdbApiService = tmdbApiService;
        _spotifyApiService = spotifyApiService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<IDataResult<List<GetMusicDto>>> GetMusicForMovieAsync(string movieTitle)
    {
        // ==========================================
        // 1. CHECK LOCAL DATABASE FIRST
        // ==========================================
        var localMovie = await _movieService.GetMovieByTitleAsync(movieTitle);
        Movie targetMovie;

        if (localMovie.Success && localMovie.Data != null)
        {
            targetMovie = _mapper.Map<Movie>(localMovie);
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
            targetMovie = (await _context.Set<Movie>()
                .Include(m => m.Categories)
                .ThenInclude(mc => mc.SuggestedMusicCategories)
                .FirstOrDefaultAsync(m => m.TmdbId == tmdbMovieDto.Data.TmdbId))!;
            
            if (targetMovie == null)
            {
                return new ErrorDataResult<List<GetMusicDto>>("Failed to retrieve saved movie.");
            }
        }

        // ==========================================
        // 3. GET LINKED MUSIC CATEGORIES
        // ==========================================
        var linkedMusicCategories = targetMovie.Categories
            .SelectMany(mc => mc.SuggestedMusicCategories)
            .Select(mc => mc.Name)
            .Distinct()
            .ToList();

        if (!linkedMusicCategories.Any())
        {
             // Fallback genre if the movie had no specific links
             linkedMusicCategories.Add("pop");
        }

        // Pick one random linked category
        var random = new Random();
        var selectedGenre = linkedMusicCategories[random.Next(linkedMusicCategories.Count)];

        // ==========================================
        // 4. GET MUSIC FROM LOCAL DB OR SPOTIFY
        // ==========================================
        
        // Let's ask Spotify for fresh tracks based on the genre
        var spotifyMusicDtos = await _spotifyApiService.FetchMusicByGenreAsync(selectedGenre, limit: 5);

        var finalMusicList = new List<GetMusicDto>();

        foreach (var newMusicDto in spotifyMusicDtos.Data)
        {
            // Check if we already have this specific Spotify ID in our DB
            var existingMusic = await _context.Set<Music>()
                .FirstOrDefaultAsync(m => m.SpotifyId == newMusicDto.SpotifyId);
            var newMusicMap = _mapper.Map<CreateMusicDto>(_mapper.Map<Music>(newMusicDto));
            if (existingMusic == null)
            {
                // Save new track using your existing CRUD service
                await _musicService.CreateMusicAsync(newMusicMap);
                
                // Map the CreateDto to a GetDto for the return list
                finalMusicList.Add(_mapper.Map<GetMusicDto>(newMusicDto));
            }
            else
            {
                // We already had it, just add it to the return list
                finalMusicList.Add(_mapper.Map<GetMusicDto>(existingMusic));
            }
        }

        // ==========================================
        // 5. RETURN SUCCESS
        // ==========================================
        return new SuccessDataResult<List<GetMusicDto>>(finalMusicList, $"Successfully generated music for {targetMovie.Title}");
    }
}