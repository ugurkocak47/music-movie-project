using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.MusicCategories
{
    public class CreateMusicCategoryDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
