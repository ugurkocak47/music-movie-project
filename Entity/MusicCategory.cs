using Core.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Entity
{
    public class MusicCategory : IEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();
    }
}
