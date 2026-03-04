using Entity;

namespace DTO.Playlists;

public class GetPlaylistDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PlaylistName { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
    public List<Music> Musics { get; set; } = null!;
    public bool IsPublic { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public bool IsDeleted { get; set; }
}