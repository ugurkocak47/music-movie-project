using Core.Utilities.Results;
using DTO.Movies;
using Entity;
using Microsoft.Extensions.Configuration;
using Service.Abstracts;
using TMDbLib.Client;

namespace Service.Concretes;

public class TmdbApiService : ITmdbApiService
{
    private readonly TMDbClient _tmdbClient;
    private readonly IMovieCategoryService _movieCategoryService;

    public TmdbApiService(IConfiguration configuration, IMovieCategoryService movieCategoryService)
    {
        var apiKey = configuration["ExternalApis:TmdbApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new ArgumentNullException( "TMDb API Key is missing in configuration.");
        }

        _tmdbClient = new TMDbClient(apiKey);
        _movieCategoryService = movieCategoryService;
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
                    // Map to entity
                    var categoryEntity = new MovieCategory
                    {
                        Id = categoryResult.Data.Id,
                        Name = categoryResult.Data.Name,
                        Description = categoryResult.Data.Description
                    };
                    movieCategories.Add(categoryEntity);
                }
            }
        }

        // 4. Map to GetMovieDto with categories
        var movieDto = new CreateMovieDto()
        {
            TmdbId = movieDetails!.Id,
            Title = movieDetails.Title!,
            NormalizedTitle = movieDetails.Title?.ToUpperInvariant() ?? string.Empty,
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