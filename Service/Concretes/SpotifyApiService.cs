using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    private readonly IMusicService _musicService;
    private readonly IMapper _mapper;

    public SpotifyApiService(IConfiguration configuration, IMusicCategoryService musicCategoryService, IMapper mapper, IMusicService musicService)
    {
        _musicCategoryService = musicCategoryService;
        _mapper = mapper;
        _musicService = musicService;
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
        Console.WriteLine("Fetching random tracks for genre...");
    
        // 1. Define the genre you want to search (e.g., mapped from a movie category)
    
        // 2. Generate a random offset to ensure different results each time
        // Note: Spotify caps the maximum offset at 1000 for searches
        var random = new Random();
        int randomOffset = random.Next(0, 500); 
    
        // 3. Create the search request using the 'genre:' filter syntax
        var searchReq = new SearchRequest(SearchRequest.Types.Track, $"genre:\"{genreName}\"")
        {
            Limit = limit, // How many random tracks you want back
            Offset = randomOffset
        };
    
        var searchResponse = await _spotifyClient.Search.Item(searchReq);
        var responseItems = searchResponse.Tracks?.Items;
        var tracksList = new List<CreateMusicDto>();            
        if (responseItems != null && responseItems.Any())
        {
            Console.WriteLine($"\nSuccess! Random '{genreName}' tracks found at offset {randomOffset}:");
            if ((await _musicCategoryService.GetMusicCategoryByNameAsync(genreName)) == null)
            {
                var mc = new CreateMusicCategoryDto()
                {
                    Name = char.ToUpper(genreName[0]) + genreName.Substring(1),

                };
                var result = await _musicCategoryService.CreateMusicCategoryAsync(mc);
                if (!result.Success)
                {
                    return new ErrorDataResult<List<CreateMusicDto>>(result.Messages);
                }

                Console.WriteLine("New music category has been added to the database.");
                var newMC = await _musicCategoryService.GetMusicCategoryByNameAsync(genreName);
            }
            foreach (var track in responseItems)
            {
                if (track == null) continue;
                
                var categoryResult = await _musicCategoryService.GetMusicCategoryByNameAsync(genreName);
                var musicCategory = _mapper.Map<MusicCategory>(categoryResult.Data);
                
                CreateMusicDto newTrack = new CreateMusicDto()
                {
                    Name = track.Name,
                    AlbumName = track.Album.Name,
                    DurationMs = track.DurationMs,
                    PreviewUrl = track.PreviewUrl ?? "No preview",
                    SpotifyId = track.Id,
                    ArtistName = track.Artists?.FirstOrDefault()?.Name ?? "Unknown Artist",
                    Categories = new List<MusicCategory> { musicCategory },
                    SpotifyUrl = track.Uri
                };
                var musicResult = await _musicService.GetMusicBySpotifyIdAsync(track.Id);
                tracksList.Add(newTrack);
                if (!musicResult.Success)
                {
                    Console.WriteLine("Music not found in database.");
                }

                var musicCreateResult = await _musicService.CreateMusicAsync(newTrack);
                if (!musicCreateResult.Success)
                {
                    return new ErrorDataResult<List<CreateMusicDto>>(musicCreateResult.Messages);
                }

                Console.WriteLine("Music added to the database");
                // Safely grab the first artist's name
                string artistName = track.Artists?.FirstOrDefault()?.Name ?? "Unknown Artist";
                Console.WriteLine($"- {track.Name} (by {artistName})");
            }

            
        }
        else
        {
            Console.WriteLine($"No tracks found for genre '{genreName}'. Try a different genre.");
        }

        return new SuccessDataResult<List<CreateMusicDto>>(tracksList,"Musics returned successfully.");
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
                        CreatedDate = DateTime.UtcNow
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