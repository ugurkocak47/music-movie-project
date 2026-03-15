using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Utilities.Results;
using DTO.MusicCategories;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using DTO.Musics;
using Service.Abstracts;
using Entity;

namespace Service.Concretes;

public class SpotifyApiService : ISpotifyApiService
{
    private readonly SpotifyClient _spotifyClient;
    private readonly IMusicCategoryService _musicCategoryService;

    public SpotifyApiService(IConfiguration configuration, IMusicCategoryService musicCategoryService)
    {
        _musicCategoryService = musicCategoryService;
        var clientId = configuration["ExternalApis:SpotifyClientId"];
        var clientSecret = configuration["ExternalApis:SpotifyClientSecret"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentNullException("Spotify credentials are missing in configuration.");
        }

        // This config automatically handles the 1-hour token expiration!
        var config = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));

        _spotifyClient = new SpotifyClient(config);
    }

    public async Task<IDataResult<List<CreateMusicDto>>> FetchMusicByGenreAsync(string genreName, int limit = 5)
    {
        var recommendedMusic = new List<CreateMusicDto>();

        try
        {
            // 1. Setup the request
            var request = new RecommendationsRequest
            {
                Limit = limit
            };
            
            // Spotify requires seed genres to be strictly lowercase
            request.SeedGenres.Add(genreName.ToLower());

            // 2. Call the Spotify API
            var response = await _spotifyClient.Browse.GetRecommendations(request);

            // 3. Map the Spotify Tracks to your CreateMusicDto
            foreach (var track in response.Tracks)
            {
                var createDto = new CreateMusicDto()
                {
                    SpotifyId = track.Id,
                    Name = track.Name,
                    // Grab the first artist's name
                    ArtistName = track.Artists.FirstOrDefault()?.Name ?? "Unknown Artist",
                    AlbumName = track.Album?.Name,
                    DurationMs = track.DurationMs,
                    PreviewUrl = track.PreviewUrl,
                    
                    // Extract the external Spotify URL so users can click and listen
                    SpotifyUrl = track.ExternalUrls.ContainsKey("spotify") 
                                 ? track.ExternalUrls["spotify"] 
                                 : null
                };

                recommendedMusic.Add(createDto);
            }
        }
        catch (Exception ex)
        {
            // If you pass a genre Spotify doesn't recognize, it will throw an API exception.
            // In a production app, you'd log this error using an ILogger.
            Console.WriteLine($"Spotify API Error: {ex.Message}");
        }

        return new SuccessDataResult<List<CreateMusicDto>>(recommendedMusic);
    }

    public async Task<IDataResult<List<CreateMusicCategoryDto>>> GetAllNewMusicGenresAsync()
    {
        var addedCategories = new List<CreateMusicCategoryDto>();
        try
        {
            // 1. Fetch genres from Spotify
            var response = await _spotifyClient.Browse.GetRecommendationGenres();
            var spotifyGenres = response.Genres;

            // 2. Fetch existing categories from DB
            var existingCategoriesResult = await _musicCategoryService.GetAllMusicCategoriesAsync();
            var existingCategoryNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (existingCategoriesResult.Success && existingCategoriesResult.Data != null)
            {
                foreach (var category in existingCategoriesResult.Data)
                {
                    existingCategoryNames.Add(category.Name);
                }
            }

            // 3. Filter and Add new genres
            foreach (var genre in spotifyGenres)
            {
                if (!existingCategoryNames.Contains(genre))
                {
                    var newCategory = new CreateMusicCategoryDto
                    {
                        Name = char.ToUpper(genre[0]) + genre.Substring(1), 
                        Description = "Imported from Spotify",
                        CreatedDate = DateTime.UtcNow,
                        LinkedMovieCategories = new List<MovieCategory>()
                    };

                    // Add to DB
                    var addResult = await _musicCategoryService.CreateMusicCategoryAsync(newCategory);

                    if (addResult.Success)
                    {
                        addedCategories.Add(newCategory);
                    }
                }
            }

            return new SuccessDataResult<List<CreateMusicCategoryDto>>(addedCategories, $"Added {addedCategories.Count} new genres from Spotify.");
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<List<CreateMusicCategoryDto>>($"Failed to sync genres: {ex.Message}");
        }
    }
}