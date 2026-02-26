using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Movies
{
    public class GetMovieDto
    {
        public Guid Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? PosterPath { get; set; }
        public float Rating { get; set; }
        public ICollection<MovieCategory> Categories { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
