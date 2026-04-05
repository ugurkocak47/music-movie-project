using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Movies
{
    public class CreateMovieDto
    {
        public int TmdbId { get; set; }
        public string Title { get; set; }
        public string NormalizedTitle { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? PosterPath { get; set; }
        
        // Full poster URL (constructed from PosterPath)
        public string? PosterUrl => !string.IsNullOrEmpty(PosterPath) 
            ? $"https://image.tmdb.org/t/p/w500{PosterPath}" 
            : null;
            
        public float Rating { get; set; }
        public ICollection<MovieCategory> Categories { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
