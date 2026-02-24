using Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class MovieCategory:IEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<MusicCategory> SuggestedMusicCategories { get; set; } = new List<MusicCategory>();
    }
}
