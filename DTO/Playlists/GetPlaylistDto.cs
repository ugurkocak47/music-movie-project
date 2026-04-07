using DTO.Movies;
using DTO.Musics;

namespace DTO.Playlists;

public class GetPlaylistDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PlaylistName { get; set; } = null!;
    public string? Description { get; set; }
    
    public Guid MovieId { get; set; }
    public GetMovieDto? Movie { get; set; }
    
    public List<GetMusicDto> Musics { get; set; } = new();
    
    public bool IsPublic { get; set; }
    public int FavoriteCount { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DeletedDate { get; set; }
    public bool IsDeleted { get; set; }
}