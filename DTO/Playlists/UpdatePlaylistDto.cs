using Entity;

namespace DTO.Playlists;

public class UpdatePlaylistDto
{
    public Guid Id { get; set; }    
    public Guid UserId { get; set; }
    public string PlaylistName { get; set; } = null!;
    public string? Description { get; set; }
    public Guid MovieId { get; set; }
    public List<Guid> MusicIds { get; set; } = null!;
    public bool IsPublic { get; set; }
}