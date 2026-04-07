using Entity;

namespace DTO.Playlists;

public class CreatePlaylistDto
{
    public Guid UserId { get; set; }
    public string PlaylistName { get; set; } = null!;
    public string? Description { get; set; }
    public Guid MovieId { get; set; } 
    public List<Guid> MusicIds { get; set; } = null!;
    public bool IsPublic { get; set; }
}