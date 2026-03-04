using Core.Service;
using Core.Utilities.Results;
using DTO.Playlists;
using Entity;

namespace Service.Abstracts;

public interface IPlaylistService:IDbOperationEvent<Playlist,Guid,AppUser,AppDbContext>
{
    public Task<IResult> CreatePlaylistAsync(CreatePlaylistDto playlistDto);
    public Task<IResult> UpdatePlaylistAsync(UpdatePlaylistDto playlistDto);
    public Task<IResult> DeletePlaylistAsync(Guid id);
    public Task<IDataResult<List<GetPlaylistDto>>> GetAllPlaylistsAsync(bool getPrivate);
    public Task<IDataResult<GetPlaylistDto>> GetPlaylistById(Guid id);
    public Task<IDataResult<List<GetPlaylistDto>>> GetPlaylistsByUserId(Guid userId, bool getPrivate);
    public Task<IDataResult<List<GetPlaylistDto>>> GetPlaylistsByMovieId(Guid movieId,bool getPrivate);
}