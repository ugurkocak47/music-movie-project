using Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity
{
    public class Movie:IEntity<Guid>
    {
        public int TmdbId { get; set; }
        public string Title { get; set; } = null!;

        public string? OriginalTitle { get; set; }
        public string NormalizedTitle { get; set; } = null!;

        // Maps to TMDb's "Overview" property
        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }
        public string? PosterPath { get; set; }

        public float Rating { get; set; } // Maps to TMDb's VoteAverage
    }
}
