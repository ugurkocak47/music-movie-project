using AutoMapper;
using Core.Aspects.Autofac.Validation;
using Core.Service;
using Core.Utilities.Results;
using DTO.Musics;
using DTO.ValidationRules;
using Entity;
using Service.Abstracts;

namespace Service.Concretes;

public class MusicService:IMusicService
{
    public ITBaseService<Music, Guid, AppUser, AppDbContext> Current { get; }
    private readonly IMapper _mapper;
    
    public MusicService(ITBaseService<Music, Guid, AppUser, AppDbContext> current, IMapper mapper)
    {
        Current = current;
        _mapper = mapper;
    }
    
    [ValidationAspect(typeof(MusicValidator))]
    public async Task<IResult> CreateMusicAsync(CreateMusicDto musicDto)
    {
        var musicMap = _mapper.Map<Music>(musicDto);
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