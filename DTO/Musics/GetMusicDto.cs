using Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Musics
{
    public class GetMusicDto
    {
        public Guid Id { get; set; }
        public string SpotifyId { get; set; }
        public string Name { get; set; }
        public string? ArtistName { get; set; }
        public string? AlbumName { get; set; }
        public string? SpotifyUrl { get; set; }
        public string? PreviewUrl { get; set; }
        public int DurationMs { get; set; }
        public ICollection<MusicCategory> Categories { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
