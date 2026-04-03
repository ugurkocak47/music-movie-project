using Core.Service;
using Core.Utilities.Results;
using DTO.Musics;
using Entity;

namespace Service.Abstracts;

public interface IMusicService:IDbOperationEvent<Music,Guid,AppUser,AppDbContext>
{
    public Task<IResult> CreateMusicAsync(CreateMusicDto musicDto, Guid? categoryId = null);
    public Task<IResult> UpdateMusicAsync(UpdateMusicDto musicDto);
    public Task<IResult> SoftDeleteMusicAsync(Guid id);
    public Task<IDataResult<List<GetMusicDto>>> GetAllMusicsAsync();
    public Task<IDataResult<GetMusicDto>> GetMusicBySpotifyIdAsync(string spotifyId);
    public Task<IDataResult<GetMusicDto>> GetMusicByIdAsync(Guid id);
}