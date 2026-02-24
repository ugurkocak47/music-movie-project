using Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class MusicCategory : IEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<MovieCategory> LinkedMovieCategories { get; set; } = new List<MovieCategory>();
    }
}
