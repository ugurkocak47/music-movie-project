using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.MovieCategories
{
    public class CreateMovieCategoryDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<MusicCategory> SuggestedMusicCategories { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
