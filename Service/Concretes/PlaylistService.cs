using AutoMapper;
using Core.Aspects.Autofac.Validation;
using Core.Service;
using Core.Utilities.Results;
using DTO.Playlists;
using DTO.ValidationRules;
using Entity;
using Service.Abstracts;

namespace Service.Concretes;

public class PlaylistService:IPlaylistService
{
    public ITBaseService<Playlist, Guid, AppUser, AppDbContext> Current { get; }
    private readonly IMapper _mapper;
    public PlaylistService(ITBaseService<Playlist, Guid, AppUser, AppDbContext> current, IMapper mapper)
    {
        Current = current;
        _mapper = mapper;
    }

    [ValidationAspect(typeof(PlaylistValidator))]
    public async Task<IResult> CreatePlaylistAsync(CreatePlaylistDto playlistDto)
    {
        var playlistMap = _mapper.Map<Playlist>(playlistDto);
        await Current.AddAsync(playlistMap);
        return new SuccessResult("Playlist created successfully.");
    }
    
    [ValidationAspect(typeof(PlaylistValidator))]
    public async Task<IResult> UpdatePlaylistAsync(UpdatePlaylistDto playlistDto)
    {
        var playlist = await Current.FirstOrDefaultAsync(p=>p.Id == playlistDto.Id);
        if (playlist == null)
        {
            return new ErrorResult($"Playlist with ID {playlistDto.Id} not found.");
        }

        var playlistMap = _mapper.Map(playlistDto, playlist);
        await Current.UpdateAsync(playlistMap);
        return new SuccessResult("Playlist updated successfully.");
    }

    public async Task<IResult> DeletePlaylistAsync(Guid id)
    {
        var playlist = await Current.FirstOrDefaultAsync(p=>p.Id == id);
        if (playlist == null)
        {
            return new ErrorResult($"Playlist with ID {id} not found.");
        }

        await Current.DeleteAsync(playlist);
        return new SuccessResult("Playlist deleted successfully.");

    }

    public async Task<IDataResult<List<GetPlaylistDto>>> GetAllPlaylistsAsync(bool getPrivate)
    {
        var playlists = getPrivate 
            ? await Current.GetAllListAsync()
            : await Current.GetAllListAsync(p => p.IsPublic);


        if (!playlists.Any())
        {
            return new ErrorDataResult<List<GetPlaylistDto>>("No playlists found.");
        }
        var playlistsMap = _mapper.Map<List<GetPlaylistDto>>(playlists);

        return new SuccessDataResult<List<GetPlaylistDto>>(playlistsMap, "Playlists returned successfully.");
    }

    public async Task<IDataResult<GetPlaylistDto>> GetPlaylistById(Guid id)
    {
        var playlist = await Current.FirstOrDefaultAsync(p => p.Id == id);
        if (playlist == null)
        {
            return new ErrorDataResult<GetPlaylistDto>($"Playlist with ID {id} not found.");
        }

        var playlistMap = _mapper.Map<GetPlaylistDto>(playlist);
        return new SuccessDataResult<GetPlaylistDto>(playlistMap, "Playlist returned successfully.");
    }

    public async Task<IDataResult<List<GetPlaylistDto>>> GetPlaylistsByUserId(Guid userId, bool getPrivate)
    {
        var playlists = getPrivate 
            ? await Current.GetAllListAsync(p=>p.UserId == userId)
            : await Current.GetAllListAsync(p => p.IsPublic && p.UserId == userId);
        if (!playlists.Any())
        {
            return new ErrorDataResult<List<GetPlaylistDto>>("No playlist found for this user.");
        }

        var playlistsMap = _mapper.Map<List<GetPlaylistDto>>(playlists);
        return new SuccessDataResult<List<GetPlaylistDto>>(playlistsMap, "Playlists returned successfully.");
    }

    public async Task<IDataResult<List<GetPlaylistDto>>> GetPlaylistsByMovieId(Guid movieId, bool getPrivate)
    {
        var playlists = getPrivate 
            ? await Current.GetAllListAsync(p=>p.Movie.Id == movieId)
            : await Current.GetAllListAsync(p => p.IsPublic && p.Movie.Id == movieId);
        if (!playlists.Any())
        {
            return new ErrorDataResult<List<GetPlaylistDto>>("No playlist found for this movie");
        }

        var playlistsMap = _mapper.Map<List<GetPlaylistDto>>(playlists);
        return new SuccessDataResult<List<GetPlaylistDto>>(playlistsMap, "Playlists returned successfully.");
    }
}