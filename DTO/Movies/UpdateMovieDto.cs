using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Movies
{
    public class UpdateMovieDto
    {
        public Guid Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; }
        public string NormalizedTitle { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? PosterPath { get; set; }
        public float Rating { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
