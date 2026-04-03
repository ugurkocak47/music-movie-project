using Core.Service;
using Core.Utilities.Results;
using DTO.MovieCategories;
using DTO.MusicCategories;
using Entity;

namespace Service.Abstracts;

public interface ICategoryLinkingService:IDbOperationEvent<MovieCategoryLink,Guid,AppUser,AppDbContext>, IDbOperationEvent<MusicCategoryLink,Guid,AppUser,AppDbContext>
{
    public Task<IResult> LinkMusicToCategoryAsync(string musicSpotifyId, Guid categoryId);
    public Task<IResult> LinkMovieToCategoryAsync(int movieTmdbId, Guid categoryId);
    public Task<IDataResult<List<GetMusicCategoryDto>>> GetMusicCategoriesAsync(Guid musicId);
    public Task<IDataResult<List<GetMovieCategoryDto>>> GetMovieCategoriesAsync(Guid movieId);
}