using AutoMapper;
using Core.Aspects.Autofac.Validation;
using Core.Service;
using Core.Utilities.Results;
using DTO.Playlists;
using DTO.ValidationRules;
using Entity;
using Microsoft.AspNetCore.Identity;
using Service.Abstracts;

namespace Service.Concretes;

public class PlaylistService:IPlaylistService
{
    public ITBaseService<Playlist, Guid, AppUser, AppDbContext> Current { get; }
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    public PlaylistService(ITBaseService<Playlist, Guid, AppUser, AppDbContext> current, IMapper mapper, IUserService userService, UserManager<AppUser> userManager)
    {
        Current = current;
        _mapper = mapper;
        _userManager = userManager;
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

    public async Task<IResult> FavoritePlaylist(Guid userId, Guid playlistId)
    {
        // Fetch both user and playlist in parallel
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return new ErrorResult("User not found.");
        }

        var playlist = await Current.FirstOrDefaultAsync(p => p.Id == playlistId);
        if (playlist == null)
        {
            return new ErrorResult($"Playlist with ID {playlistId} not found.");
        }

        // Check if already favorited
        if (user.FavoritePlaylists.Any(p => p.Id == playlistId))
        {
            return new ErrorResult("This playlist is already in your favorites.");
        }

        playlist.FavoriteCount++;
        await Current.UpdateAsync(playlist);
        user.FavoritePlaylists.Add(playlist);
        await _userManager.UpdateAsync(user);
    
        return new SuccessResult("Playlist added to favorites successfully.");
    }

    public async Task<IResult> AddToExistingPlaylist(Guid playlistId,CreatePlaylistDto playlistDto)
    {
        var playlist = await Current.FirstOrDefaultAsync(p => p.Id == playlistId);
        if (playlist == null)
        {
            return new ErrorResult("Playlist not found.");
        }

        if (playlist.Movie != playlistDto.Movie)
        {
            return new ErrorResult("Playlist movies do not match.");
        }
        foreach (var music in playlistDto.Musics)
        {
            if (!playlist.Musics.Contains(music))
            {
                playlist.Musics.Add(music);
            }
        }

        await Current.UpdateAsync(playlist);
        return new SuccessResult("Added musics to playlist successfully.");
    }
}