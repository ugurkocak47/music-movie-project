using Entity;

namespace DTO.Playlists;

public class UpdatePlaylistDto
{
    public Guid Id { get; set; }    
    public Guid UserId { get; set; }
    public string PlaylistName { get; set; } = null!;
    public string? Description { get; set; }
    public Movie Movie { get; set; } = null!;
    public List<Music> Musics { get; set; } = null!;
    public bool IsPublic { get; set; }
    public DateTime UpdatedDate { get; set; }
}