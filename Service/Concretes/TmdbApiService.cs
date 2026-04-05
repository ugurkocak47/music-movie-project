using Core.Helpers;
using Core.Utilities.Results;
using DTO.Movies;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Service.Abstracts;
using TMDbLib.Client;

namespace Service.Concretes;

public class TmdbApiService : ITmdbApiService
{
    private readonly TMDbClient _tmdbClient;
    private readonly IMovieCategoryService _movieCategoryService;
    private readonly AppDbContext _context;

    public TmdbApiService(
        IConfiguration configuration, 
        IMovieCategoryService movieCategoryService,
        AppDbContext context)
    {
        var apiKey = configuration["ExternalApis:TmdbApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentNullException( "TMDb API Key is missing in configuration.");
        }

        _tmdbClient = new TMDbClient(apiKey);
        _movieCategoryService = movieCategoryService;
        _context = context;
    }

    public async Task<IDataResult<CreateMovieDto>> FetchMovieFromTmdbAsync(string movieTitle)
    {
        // 1. Search for the movie
        var searchResults = await _tmdbClient.SearchMovieAsync(movieTitle);
        var firstResult = searchResults.Results.FirstOrDefault();

        if (firstResult == null)
        {
            return new ErrorDataResult<CreateMovieDto>("Movie not found.");
        }

        // 2. Fetch full details to get genres
        var movieDetails = await _tmdbClient.GetMovieAsync(firstResult.Id);

        // 3. Process genres and get/create MovieCategory entities
        var movieCategories = new List<MovieCategory>();
        if (movieDetails.Genres != null && movieDetails.Genres.Any())
        {
            foreach (var genre in movieDetails.Genres)
            {
                var categoryResult = await _movieCategoryService.GetOrCreateMovieCategoryByNameAsync(genre.Name);
                if (categoryResult.Success && categoryResult.Data != null)
                {
                    // Fetch the actual entity from database without tracking
                    // This prevents EF Core tracking conflicts
                    var categoryEntity = await _context.Set<MovieCategory>()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(mc => mc.Id == categoryResult.Data.Id);
                    
                    if (categoryEntity != null)
                    {
                        movieCategories.Add(categoryEntity);
                    }
                }
            }
        }

        // 4. Map to GetMovieDto with categories
        var movieDto = new CreateMovieDto()
        {
            TmdbId = movieDetails!.Id,
            Title = movieDetails.Title!,
            NormalizedTitle = StringHelper.NormalizeTurkish(movieDetails.Title ?? string.Empty),
            Description = movieDetails.Overview,
            ReleaseDate = movieDetails.ReleaseDate,
            PosterPath = movieDetails.PosterPath,
            Rating = (float)movieDetails.VoteAverage,
            Categories = movieCategories, // ✅ Now populated with actual genres!
            CreatedDate = DateTime.UtcNow,
        };

        return new SuccessDataResult<CreateMovieDto>(movieDto, "Movie fetched successfully from TMDb.");
    }
}