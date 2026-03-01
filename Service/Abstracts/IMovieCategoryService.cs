using Core.Service;
using Core.Utilities.Results;
using DTO.MovieCategories;
using Entity;

namespace Service.Abstracts;

public interface IMovieCategoryService:IDbOperationEvent<MovieCategory,Guid,AppUser,AppDbContext>
{
    public Task<IResult> CreateMovieCategoryAsync(CreateMovieCategoryDto movieCategoryDto);
    public Task<IResult> UpdateMovieCategoryAsync(UpdateMovieCategoryDto movieCategoryDto);
    public Task<IDataResult<List<GetMovieCategoryDto>>> GetAllMovieCategoriesAsync();
    public Task<IDataResult<GetMovieCategoryDto>> GetMovieCategoryByIdAsync(Guid id);
    public Task<IResult> SoftDeleteMovieCategoryAsync(Guid id);
}
