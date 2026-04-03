using System;
using System.Collections.Generic;
using System.Linq;

namespace Service.Helpers;

/// <summary>
/// Maps movie genres to suggested music genres.
/// Provides comprehensive mappings where each movie genre links to 2-4 relevant music genres.
/// </summary>
public static class GenreMappingHelper
{
    // Comprehensive mapping: Movie Genre → Multiple Music Genres
    // Each movie genre maps to 2-4 music genres that match its mood/vibe
    private static readonly Dictionary<string, List<string>> GenreMappings = new()
    {
        // Action & Adventure - High energy, intense, epic
        { "action", new() { "rock", "metal", "electronic", "hip-hop" } },
        { "adventure", new() { "rock", "indie", "folk", "world" } },
        
        // Comedy & Light-hearted - Upbeat, fun, cheerful
        { "comedy", new() { "pop", "indie", "funk", "soul" } },
        { "family", new() { "pop", "indie", "folk", "country" } },
        { "animation", new() { "pop", "indie", "electronic" } },
        
        // Drama & Emotional - Deep, introspective, moving
        { "drama", new() { "indie", "folk", "classical", "jazz" } },
        { "romance", new() { "pop", "r&b", "soul", "indie" } },
        
        // Thriller & Suspense - Dark, tense, mysterious
        { "thriller", new() { "electronic", "rock", "indie" } },
        { "crime", new() { "hip-hop", "jazz", "electronic", "blues" } },
        { "mystery", new() { "jazz", "classical", "indie", "electronic" } },
        
        // Horror & Dark - Ominous, heavy, unsettling
        { "horror", new() { "metal", "rock", "electronic" } },
        
        // Sci-Fi & Fantasy - Otherworldly, futuristic, epic
        { "sci-fi", new() { "electronic", "rock", "indie" } },
        { "science fiction", new() { "electronic", "rock", "indie" } },
        { "fantasy", new() { "folk", "world", "classical", "indie" } },
        
        // Historical & Cultural - Period-appropriate, traditional
        { "war", new() { "classical", "rock", "metal", "folk" } },
        { "history", new() { "classical", "folk", "world", "jazz" } },
        { "western", new() { "country", "folk", "rock", "blues" } },
        { "period", new() { "classical", "jazz", "folk" } },
        
        // Biographical & Real Stories - Inspiring, reflective
        { "biography", new() { "indie", "folk", "rock", "jazz" } },
        { "biopic", new() { "indie", "folk", "rock", "jazz" } },
        
        // Sports & Competition - Motivational, energetic
        { "sport", new() { "rock", "hip-hop", "electronic", "pop" } },
        { "sports", new() { "rock", "hip-hop", "electronic", "pop" } },
        
        // Music & Performance - Genre-spanning
        { "music", new() { "pop", "rock", "indie", "jazz" } },
        { "musical", new() { "pop", "jazz", "indie", "soul" } },
        
        // Documentary & Educational - Thoughtful, atmospheric
        { "documentary", new() { "indie", "classical", "world", "jazz" } },
        { "news", new() { "jazz", "classical", "indie" } },
        
        // Noir & Detective - Moody, atmospheric
        { "noir", new() { "jazz", "blues", "indie" } },
        { "detective", new() { "jazz", "blues", "electronic" } },
        
        // Disaster & Survival - Intense, dramatic
        { "disaster", new() { "rock", "electronic", "classical" } },
        { "survival", new() { "rock", "folk", "indie" } },
        
        // Supernatural & Paranormal - Eerie, mysterious
        { "supernatural", new() { "electronic", "indie", "rock" } },
        { "paranormal", new() { "electronic", "indie", "metal" } },
        
        // Teen & Young Adult - Contemporary, relatable
        { "teen", new() { "pop", "indie", "hip-hop", "rock" } },
        { "coming of age", new() { "indie", "pop", "rock", "folk" } },
        { "coming-of-age", new() { "indie", "pop", "rock", "folk" } },
        
        // Catch-all for TV Movies and Others
        { "tv movie", new() { "pop", "indie", "rock" } }
    };

    /// <summary>
    /// Gets suggested music genres for a given movie genre.
    /// Returns multiple genres for variety - RecommendationService will pick one randomly.
    /// </summary>
    /// <param name="movieGenre">The movie genre name (case-insensitive)</param>
    /// <returns>List of 2-4 suggested music genre names, or default fallback if not mapped</returns>
    public static List<string> GetSuggestedMusicGenres(string movieGenre)
    {
        if (string.IsNullOrWhiteSpace(movieGenre))
        {
            return GetDefaultGenres();
        }

        var normalized = movieGenre.Trim().ToLowerInvariant();
        
        if (GenreMappings.TryGetValue(normalized, out var musicGenres))
        {
            // Return a copy of the list to prevent modifications to the original
            return new List<string>(musicGenres);
        }
        
        // Default fallback for unmapped genres
        return GetDefaultGenres();
    }
    
    /// <summary>
    /// Checks if a movie genre has a defined mapping.
    /// </summary>
    /// <param name="movieGenre">The movie genre name (case-insensitive)</param>
    /// <returns>True if the genre has a mapping, false otherwise</returns>
    public static bool HasMapping(string movieGenre)
    {
        if (string.IsNullOrWhiteSpace(movieGenre))
        {
            return false;
        }
        
        return GenreMappings.ContainsKey(movieGenre.Trim().ToLowerInvariant());
    }
    
    /// <summary>
    /// Gets all available movie genres that have mappings.
    /// </summary>
    /// <returns>Read-only list of movie genre names</returns>
    public static IReadOnlyList<string> GetAllMovieGenres()
    {
        return GenreMappings.Keys.ToList().AsReadOnly();
    }
    
    /// <summary>
    /// Gets the total number of defined mappings.
    /// </summary>
    /// <returns>Number of movie genres with mappings</returns>
    public static int GetMappingCount()
    {
        return GenreMappings.Count;
    }
    
    /// <summary>
    /// Gets all unique music genres used across all mappings.
    /// </summary>
    /// <returns>Distinct list of music genre names</returns>
    public static IReadOnlyList<string> GetAllMusicGenres()
    {
        return GenreMappings.Values
            .SelectMany(genres => genres)
            .Distinct()
            .OrderBy(g => g)
            .ToList()
            .AsReadOnly();
    }
    
    /// <summary>
    /// Default fallback genres for unmapped movie genres.
    /// </summary>
    private static List<string> GetDefaultGenres()
    {
        return new List<string> { "pop", "indie", "rock" };
    }
}
