using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Utilities.Results;
using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using DTO.Musics;
using Service.Abstracts;

namespace Service.Concretes;

public class SpotifyApiService : ISpotifyApiService
{
    private readonly SpotifyClient _spotifyClient;

    public SpotifyApiService(IConfiguration configuration)
    {
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
}