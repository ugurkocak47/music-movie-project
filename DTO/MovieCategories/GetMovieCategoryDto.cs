using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.MovieCategories
{
    public class GetMovieCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<MusicCategory> SuggestedMusicCategories { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
