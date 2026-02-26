using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.MusicCategories
{
    public class UpdateMusicCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public ICollection<MovieCategory> LinkedMovieCategories { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
