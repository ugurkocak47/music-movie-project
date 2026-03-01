using AutoMapper;
using Core.Aspects.Autofac.Validation;
using Core.Service;
using Core.Utilities.Results;
using DTO.MusicCategories;
using DTO.ValidationRules;
using Entity;
using Service.Abstracts;

namespace Service.Concretes;

public class MusicCategoryService:IMusicCategoryService
{
    public MusicCategoryService(IMapper mapper, ITBaseService<MusicCategory, Guid, AppUser, AppDbContext> current)
    {
        _mapper = mapper;
        Current = current;
    }

    public ITBaseService<MusicCategory, Guid, AppUser, AppDbContext> Current { get; }
    private readonly IMapper _mapper;
    
    [ValidationAspect(typeof(MusicCategoryValidator))]
    public async Task<IResult> CreateMusicCategoryAsync(CreateMusicCategoryDto musicCategoryDto)
    {
        var musicCategoryMap = _mapper.Map<MusicCategory>(musicCategoryDto);
        await Current.AddAsync(musicCategoryMap);
        return new SuccessResult("Music category added successfully.");
    }
    
    [ValidationAspect(typeof(MusicCategoryValidator))]
    public async Task<IResult> UpdateMusicCategoryAsync(UpdateMusicCategoryDto musicCategoryDto)
    {
        var category = await Current.FirstOrDefaultAsync(c => c.Id == musicCategoryDto.Id);
        if (category == null)
        {
            return new ErrorResult($"Music category with ID {musicCategoryDto.Id} not found.");
        }

        var categoryMap = _mapper.Map<MusicCategory>(musicCategoryDto);
        await Current.UpdateAsync(categoryMap);
        return new SuccessResult("Music category updated successfully.");
    }

    public async Task<IDataResult<List<GetMusicCategoryDto>>> GetAllMusicCategoriesAsync()
    {
        var categories = await Current.GetAllListAsync();
        if (!categories.Any())
        {
            return new ErrorDataResult<List<GetMusicCategoryDto>>("No music categories found.");
        }

        var categoriesMap = _mapper.Map<List<GetMusicCategoryDto>>(categories);
        return new SuccessDataResult<List<GetMusicCategoryDto>>(categoriesMap,
            "Music categories returned successfully.");
    }

    public async Task<IDataResult<GetMusicCategoryDto>> GetMusicCategoryByIdAsync(Guid id)
    {
        var category = await Current.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return new ErrorDataResult<GetMusicCategoryDto>($"Music category with ID {id} not found.");
        }

        var categoryMap = _mapper.Map<GetMusicCategoryDto>(category);
        return new SuccessDataResult<GetMusicCategoryDto>(categoryMap, "Music category returned successfully.");
    }

    public async Task<IResult> SoftDeleteMusicCategoryAsync(Guid id)
    {
        var category = await Current.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return new ErrorResult($"Music category with ID {id} not found.");
        }

        await Current.DeleteAsync(category);
        return new SuccessResult("Music category deleted successfully.");
    }
}