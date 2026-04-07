using System.ComponentModel.DataAnnotations;
using Core.Entity;

namespace Entity;

public class Playlist:IEntity<Guid>
{
    public Guid UserId { get; set; }
    public string PlaylistName { get; set; } = null!;
    public string? Description { get; set; }
    
    // Navigation properties for EF Core
    public Guid MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    
    public List<Music> Musics { get; set; } = null!;
    
    public bool IsPublic { get; set; }
    public int FavoriteCount { get; set; } = 0;
}