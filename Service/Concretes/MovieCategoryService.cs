using AutoMapper;
using Core.Aspects.Autofac.Validation;
using Core.Service;
using Core.Utilities.Results;
using DTO.MovieCategories;
using DTO.ValidationRules;
using Entity;
using Service.Abstracts;

namespace Service.Concretes;

public class MovieCategoryService:IMovieCategoryService
{
    public MovieCategoryService(IMapper mapper, ITBaseService<MovieCategory, Guid, AppUser, AppDbContext> current)
    {
        _mapper = mapper;
        Current = current;
    }

    public ITBaseService<MovieCategory, Guid, AppUser, AppDbContext> Current { get; }
    private readonly IMapper _mapper;
    
    [ValidationAspect(typeof(MovieCategoryValidator))]
    public async Task<IResult> CreateMovieCategoryAsync(CreateMovieCategoryDto movieCategoryDto)
    {
        var movieCategoryMap = _mapper.Map<MovieCategory>(movieCategoryDto);
        await Current.AddAsync(movieCategoryMap);
        return new SuccessResult("Movie category added successfully.");
    }
    
    [ValidationAspect(typeof(MovieCategoryValidator))]
    public async Task<IResult> UpdateMovieCategoryAsync(UpdateMovieCategoryDto movieCategoryDto)
    {
        var category = await Current.FirstOrDefaultAsync(c => c.Id == movieCategoryDto.Id);
        if (category == null)
        {
            return new ErrorResult($"Movie category with ID {movieCategoryDto.Id} not found.");
        }

        var categoryMap = _mapper.Map<MovieCategory>(movieCategoryDto);
        await Current.UpdateAsync(categoryMap);
        return new SuccessResult("Movie category updated successfully.");
    }

    public async Task<IDataResult<List<GetMovieCategoryDto>>> GetAllMovieCategoriesAsync()
    {
        var categories = await Current.GetAllListAsync();
        if (!categories.Any())
        {
            return new ErrorDataResult<List<GetMovieCategoryDto>>("No categories found.");
        }

        var categoriesMap = _mapper.Map<List<GetMovieCategoryDto>>(categories);
        return new SuccessDataResult<List<GetMovieCategoryDto>>(categoriesMap,
            "Movie categories returned successfully.");
    }

    public async Task<IDataResult<GetMovieCategoryDto>> GetMovieCategoryByIdAsync(Guid id)
    {
        var category = await Current.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return new ErrorDataResult<GetMovieCategoryDto>($"Movie category with ID {id} not found.");
        }

        var categoryMap = _mapper.Map<GetMovieCategoryDto>(category);
        return new SuccessDataResult<GetMovieCategoryDto>(categoryMap, "Movie category returned successfully.");
    }

    public async Task<IResult> SoftDeleteMovieCategoryAsync(Guid id)
    {
        var category = await Current.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return new ErrorResult($"Movie category with ID {id} not found");
        }

        await Current.DeleteAsync(category);
        return new SuccessResult("Movie category deleted successfully.");
    }
}
