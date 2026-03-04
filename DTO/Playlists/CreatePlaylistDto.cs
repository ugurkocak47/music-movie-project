using Entity;

namespace DTO.Playlists;

public class CreatePlaylistDto
{
    public Guid UserId { get; set; }
    public string PlaylistName { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
    public List<Music> Musics { get; set; } = null!;
    public bool IsPublic { get; set; }
    public DateTime CreatedDate { get; set; }
}