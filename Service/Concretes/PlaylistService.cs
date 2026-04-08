using System.Linq.Expressions;
using AutoMapper;
using Core.Aspects.Autofac.Validation;
using Core.Service;
using Core.Utilities.Results;
using DTO.Playlists;
using DTO.ValidationRules;
using Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Abstracts;

namespace Service.Concretes;

public class PlaylistService:IPlaylistService
{
    public ITBaseService<Playlist, Guid, AppUser, AppDbContext> Current { get; }
    private readonly ITBaseService<Movie, Guid, AppUser, AppDbContext> _movieService;
    private readonly ITBaseService<Music, Guid, AppUser, AppDbContext> _musicService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _dbContext;
    
    public PlaylistService(
        ITBaseService<Playlist, Guid, AppUser, AppDbContext> current, 
        ITBaseService<Movie, Guid, AppUser, AppDbContext> movieService,
        ITBaseService<Music, Guid, AppUser, AppDbContext> musicService,
        IMapper mapper, 
        IUserService userService, 
        UserManager<AppUser> userManager,
        AppDbContext dbContext)
    {
        Current = current;
        _movieService = movieService;
        _musicService = musicService;
        _mapper = mapper;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    [ValidationAspect(typeof(PlaylistValidator))]
    public async Task<IResult> CreatePlaylistAsync(CreatePlaylistDto playlistDto)
    {
        // Fetch existing Movie
        var movie = await _movieService.FirstOrDefaultAsync(m => m.Id == playlistDto.MovieId);
        if (movie == null)
        {
            return new ErrorResult($"Movie with ID {playlistDto.MovieId} not found.");
        }

        // Fetch existing Musics
        var musics = new List<Music>();
        foreach (var musicId in playlistDto.MusicIds)
        {
            var music = await _musicService.FirstOrDefaultAsync(m => m.Id == musicId);
            if (music == null)
            {
                return new ErrorResult($"Music with ID {musicId} not found.");
            }
            musics.Add(music);
        }

        // Create Playlist with existing entities
        var playlist = new Playlist
        {
            UserId = playlistDto.UserId,
            PlaylistName = playlistDto.PlaylistName,
            Description = playlistDto.Description,
            MovieId = movie.Id,
            Movie = movie,
            Musics = musics,
            IsPublic = playlistDto.IsPublic
        };

        await Current.AddAsync(playlist);
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

        // Update basic properties
        playlist.PlaylistName = playlistDto.PlaylistName;
        playlist.Description = playlistDto.Description;
        playlist.IsPublic = playlistDto.IsPublic;

        // Update Movie if changed
        if (playlist.MovieId != playlistDto.MovieId)
        {
            var movie = await _movieService.FirstOrDefaultAsync(m => m.Id == playlistDto.MovieId);
            if (movie == null)
            {
                return new ErrorResult($"Movie with ID {playlistDto.MovieId} not found.");
            }
            playlist.MovieId = movie.Id;
            playlist.Movie = movie;
        }

        // Update Musics
        var musics = new List<Music>();
        foreach (var musicId in playlistDto.MusicIds)
        {
            var music = await _musicService.FirstOrDefaultAsync(m => m.Id == musicId);
            if (music == null)
            {
                return new ErrorResult($"Music with ID {musicId} not found.");
            }
            musics.Add(music);
        }
        playlist.Musics = musics;

        await Current.UpdateAsync(playlist);
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
        var playlistsQuery = _dbContext.Playlists
            .Include(p => p.Movie)
            .Include(p => p.Musics)
            .AsQueryable();

        if (!getPrivate)
        {
            playlistsQuery = playlistsQuery.Where(p => p.IsPublic);
        }

        var playlists = await playlistsQuery.ToListAsync();

        if (!playlists.Any())
        {
            return new ErrorDataResult<List<GetPlaylistDto>>("No playlists found.");
        }
        
        var playlistsMap = _mapper.Map<List<GetPlaylistDto>>(playlists);
        return new SuccessDataResult<List<GetPlaylistDto>>(playlistsMap, "Playlists returned successfully.");
    }

    public async Task<IDataResult<GetPlaylistDto>> GetPlaylistById(Guid id)
    {
        var playlist = await _dbContext.Playlists
            .Include(p => p.Movie)
            .Include(p => p.Musics)
            .FirstOrDefaultAsync(p => p.Id == id);
            
        if (playlist == null)
        {
            return new ErrorDataResult<GetPlaylistDto>($"Playlist with ID {id} not found.");
        }

        var playlistMap = _mapper.Map<GetPlaylistDto>(playlist);
        return new SuccessDataResult<GetPlaylistDto>(playlistMap, "Playlist returned successfully.");
    }

    public async Task<IDataResult<List<GetPlaylistDto>>> GetPlaylistsByUserId(Guid userId, bool getPrivate)
    {
        var playlistsQuery = _dbContext.Playlists
            .Include(p => p.Movie)
            .Include(p => p.Musics)
            .Where(p => p.UserId == userId);

        if (!getPrivate)
        {
            playlistsQuery = playlistsQuery.Where(p => p.IsPublic);
        }

        var playlists = await playlistsQuery.ToListAsync();
        
        if (!playlists.Any())
        {
            return new ErrorDataResult<List<GetPlaylistDto>>("No playlist found for this user.");
        }

        var playlistsMap = _mapper.Map<List<GetPlaylistDto>>(playlists);
        return new SuccessDataResult<List<GetPlaylistDto>>(playlistsMap, "Playlists returned successfully.");
    }

    public async Task<IDataResult<List<GetPlaylistDto>>> GetPlaylistsByMovieId(Guid movieId, bool getPrivate)
    {
        var playlistsQuery = _dbContext.Playlists
            .Include(p => p.Movie)
            .Include(p => p.Musics)
            .Where(p => p.MovieId == movieId);

        if (!getPrivate)
        {
            playlistsQuery = playlistsQuery.Where(p => p.IsPublic);
        }

        var playlists = await playlistsQuery.ToListAsync();
        
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

        if (playlist.MovieId != playlistDto.MovieId)
        {
            return new ErrorResult("Playlist movies do not match.");
        }
        
        // Fetch and add new musics
        foreach (var musicId in playlistDto.MusicIds)
        {
            if (!playlist.Musics.Any(m => m.Id == musicId))
            {
                var music = await _musicService.FirstOrDefaultAsync(m => m.Id == musicId);
                if (music != null)
                {
                    playlist.Musics.Add(music);
                }
            }
        }

        await Current.UpdateAsync(playlist);
        return new SuccessResult("Added musics to playlist successfully.");
    }

    public async Task<IDataResult<List<GetPlaylistDto>>> GetRecentPlaylists(int count, bool getPrivate)
    {
        
        var playlists =  (await Current.GetAllListAsync(m=>getPrivate ? m==m : m.IsPublic==true )).OrderByDescending(m => m.CreatedDate).Take(count);
        if (!playlists.Any())
        {
            return new ErrorDataResult<List<GetPlaylistDto>>("No playlists found.");
        }

        var playlistsMap = _mapper.Map<List<GetPlaylistDto>>(playlists);
        return new SuccessDataResult<List<GetPlaylistDto>>(playlistsMap, "Recent playlists returned successfully.");
    }
    
    private static Expression<Func<Playlist, bool>> GetPrivacyFilter(bool getPrivate)
    {
        return getPrivate ? p => true : p => p.IsPublic;
    }
}