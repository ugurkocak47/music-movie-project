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
        if (!string.IsNullOrEmpty(musicCategoryMap.Name))
        {
            musicCategoryMap.Name = char.ToUpper(musicCategoryMap.Name[0]) + musicCategoryMap.Name.Substring(1);
        }
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

        // Map DTO properties onto the existing tracked entity
        _mapper.Map(musicCategoryDto, category);
        if (!string.IsNullOrEmpty(category.Name))
        {
            category.Name = char.ToUpper(category.Name[0]) + category.Name.Substring(1);
        }
        await Current.UpdateAsync(category);
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

    public async Task<IDataResult<GetMusicCategoryDto>> GetMusicCategoryByNameAsync(string name)
    {
        name = char.ToUpper(name[0]) + name.Substring(1);
        var mc = await Current.FirstOrDefaultAsync(mc => mc.Name == name);
        if (mc == null)
        {
            return new ErrorDataResult<GetMusicCategoryDto>("Music category not found.");
        }

        var mcMap = _mapper.Map<GetMusicCategoryDto>(mc);
        return new SuccessDataResult<GetMusicCategoryDto>(mcMap, "Music category returned successfully.");
    }

    public async Task<IDataResult<GetMusicCategoryDto>> GetOrCreateMusicCategoryByNameAsync(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return new ErrorDataResult<GetMusicCategoryDto>("Category name cannot be empty.");
        }

        var normalizedName = categoryName.Trim().ToLowerInvariant();
        
        // Try to find existing category (case-insensitive)
        var existingCategory = await Current.FirstOrDefaultAsync(c => 
            c.Name.ToLower() == normalizedName);
        
        if (existingCategory != null)
        {
            var existingDto = _mapper.Map<GetMusicCategoryDto>(existingCategory);
            return new SuccessDataResult<GetMusicCategoryDto>(existingDto, "Music category found.");
        }
        
        // Create new category if not found
        var newCategoryDto = new CreateMusicCategoryDto
        {
            Name = char.ToUpper(categoryName[0]) + categoryName.Substring(1),
            Description = $"Music genre: {categoryName}"
        };
        
        var createResult = await CreateMusicCategoryAsync(newCategoryDto);
        if (!createResult.Success)
        {
            return new ErrorDataResult<GetMusicCategoryDto>("Failed to create music category.");
        }
        
        // Fetch the newly created category
        var newCategory = await Current.FirstOrDefaultAsync(c => c.Name.ToLower() == normalizedName);
        if (newCategory == null)
        {
            return new ErrorDataResult<GetMusicCategoryDto>("Failed to retrieve created music category.");
        }
        
        var newDto = _mapper.Map<GetMusicCategoryDto>(newCategory);
        return new SuccessDataResult<GetMusicCategoryDto>(newDto, "Music category created successfully.");
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