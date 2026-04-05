using Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Music:IEntity<Guid>
    {
        public string SpotifyId { get; set; } = null!;

        // Maps to Spotify's track Name
        public string Name { get; set; } = null!;

        // Spotify returns an array of artists, but for a flat database, 
        // joining primary artist names into a single string is often easiest for MVP.
        public string? ArtistName { get; set; }

        public string? AlbumName { get; set; }

        // The URL to open the song directly in the Spotify app/web player
        public string? SpotifyUrl { get; set; }

        // Spotify often provides a 30-second MP3 preview URL
        public string? PreviewUrl { get; set; }

        // Album cover image URL
        public string? AlbumImageUrl { get; set; }

        public int DurationMs { get; set; } // Spotify returns duration in milliseconds
        
    }
}
