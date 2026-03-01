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
    public Task<IResult> SoftDeleteMusicCategoryAsync(Guid id);
}