using Core.Service;
using Core.Utilities.Results;
using DTO.MusicCategories;
using Entity;

namespace Service.Abstracts;

public interface IMusicCategoryService:IDbOperationEvent<MusicCategory,Guid,AppUser,AppDbContext>
{
    public Task<IResult> CreateMusicCategoryAsync(CreateMusicCategoryDto musicCategoryDto);
    public Task<IResult> UpdateMusicCategoryAsync(UpdateMusicCategoryDto musicCategoryDto);
    public Task<IDataResult<List<GetMusicCategoryDto>>> GetAllMusicCategoriesAsync();
    
    public Task<IDataResult<GetMusicCategoryDto>> GetMusicCategoryByIdAsync(Guid id);
    public Task<IDataResult<GetMusicCategoryDto>> GetMusicCategoryByNameAsync(string name);
    public Task<IDataResult<GetMusicCategoryDto>> GetOrCreateMusicCategoryByNameAsync(string categoryName);
    public Task<IResult> SoftDeleteMusicCategoryAsync(Guid id);
}