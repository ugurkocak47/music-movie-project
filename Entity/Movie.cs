using Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Movie:IEntity<Guid>
    {

        // The ID from TMDbLib (Crucial for preventing duplicates and fetching updates later)
        public int TmdbId { get; set; }
        public string Title { get; set; } = null!;

        public string? OriginalTitle { get; set; }

        // Maps to TMDb's "Overview" property
        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        // TMDb returns image paths (e.g., "/kqjL17yufvn9OVLyXYpvtyrFfak.jpg"). 
        // You append this to their base URL to show the image in your UI.
        public string? PosterPath { get; set; }

        public double Rating { get; set; } // Maps to TMDb's VoteAverage

        // Navigation Property: A movie can have multiple categories (Genres)
        public ICollection<MovieCategory> Categories { get; set; } = new List<MovieCategory>();
    }
}
