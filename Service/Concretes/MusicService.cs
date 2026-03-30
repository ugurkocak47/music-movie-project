using AutoMapper;
using Core.Aspects.Autofac.Validation;
using Core.Service;
using Core.Utilities.Results;
using DTO.Musics;
using DTO.ValidationRules;
using Entity;
using Microsoft.EntityFrameworkCore;
using Service.Abstracts;

namespace Service.Concretes;

public class MusicService:IMusicService
{
    public ITBaseService<Music, Guid, AppUser, AppDbContext> Current { get; }
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    
    public MusicService(
        ITBaseService<Music, Guid, AppUser, AppDbContext> current, 
        IMapper mapper,
        AppDbContext context)
    {
        Current = current;
        _mapper = mapper;
        _context = context;
    }
    
    [ValidationAspect(typeof(MusicValidator))]
    public async Task<IResult> CreateMusicAsync(CreateMusicDto musicDto)
    {
        var musicMap = _mapper.Map<Music>(musicDto);
        
        // Handle existing MusicCategory entities to prevent tracking conflicts
        if (musicMap.Categories != null && musicMap.Categories.Any())
        {
            var categoryList = musicMap.Categories.ToList();
            musicMap.Categories.Clear();
            
            foreach (var category in categoryList)
            {
                // Check if this category is already being tracked
                var trackedCategory = _context.ChangeTracker.Entries<MusicCategory>()
                    .FirstOrDefault(e => e.Entity.Id == category.Id);
                
                if (trackedCategory != null)
                {
                    // Use the already-tracked entity
                    musicMap.Categories.Add(trackedCategory.Entity);
                }
                else
                {
                    // Attach as unchanged (existing entity)
                    _context.Entry(category).State = EntityState.Unchanged;
                    musicMap.Categories.Add(category);
                }
            }
        }
        
        await Current.AddAsync(musicMap);
        return new SuccessResult("Music added successfully.");
    }
    
    [ValidationAspect(typeof(MusicValidator))]
    public async Task<IResult> UpdateMusicAsync(UpdateMusicDto musicDto)
    {
        var music = await Current.FirstOrDefaultAsync(m => m.Id == musicDto.Id);
        if (music == null)
        {
            return new ErrorResult($"Music with ID {musicDto.Id} not found.");
        }

        // Map DTO properties onto the existing tracked entity
        _mapper.Map(musicDto, music);
        await Current.UpdateAsync(music);
        return new SuccessResult("Music updated successfully.");
    }

    public async Task<IResult> SoftDeleteMusicAsync(Guid id)
    {
        var music = await Current.FirstOrDefaultAsync(m => m.Id == id);
        if (music == null)
        {
            return new ErrorResult($"Music with ID {id} not found.");
        }

        await Current.DeleteAsync(music);
        return new SuccessResult("Music deleted successfully.");
    }

    public async Task<IDataResult<List<GetMusicDto>>> GetAllMusicsAsync()
    {
        var musics = await Current.GetAllListAsync();
        if (!musics.Any())
        {
            return new ErrorDataResult<List<GetMusicDto>>("No musics found.");
        }

        var musicsMap = _mapper.Map<List<GetMusicDto>>(musics);
        return new SuccessDataResult<List<GetMusicDto>>(musicsMap, "Musics returned successfully.");
    }

    public async Task<IDataResult<GetMusicDto>> GetMusicBySpotifyIdAsync(string spotifyId)
    {
        var music = await Current.FirstOrDefaultAsync(m=>m.SpotifyId == spotifyId);
        if (music == null)
        {
            return new ErrorDataResult<GetMusicDto>("Music not found in database.");
        }

        var musicMap = _mapper.Map<GetMusicDto>(music);
        return new SuccessDataResult<GetMusicDto>(musicMap, "Music returned successfully.");
    }

    public async Task<IDataResult<GetMusicDto>> GetMusicByIdAsync(Guid id)
    {
        var music = await Current.FirstOrDefaultAsync(m => m.Id == id);
        if (music == null)
        {
            return new ErrorDataResult<GetMusicDto>($"Music with ID {id} not found.");
        }

        var musicMap = _mapper.Map<GetMusicDto>(music);
        return new SuccessDataResult<GetMusicDto>(musicMap, "Music returned successfully.");
    }

}