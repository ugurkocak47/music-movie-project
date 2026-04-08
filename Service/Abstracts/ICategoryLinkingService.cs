using Core.Service;
using Core.Utilities.Results;
using DTO.MovieCategories;
using DTO.Movies;
using DTO.MusicCategories;
using DTO.Musics;
using Entity;

namespace Service.Abstracts;

public interface ICategoryLinkingService:IDbOperationEvent<MovieCategoryLink,Guid,AppUser,AppDbContext>, IDbOperationEvent<MusicCategoryLink,Guid,AppUser,AppDbContext>
{
    public Task<IResult> LinkMusicToCategoryAsync(string musicSpotifyId, Guid categoryId);
    public Task<IResult> LinkMovieToCategoryAsync(int movieTmdbId, Guid categoryId);
    public Task<IDataResult<List<GetMusicCategoryDto>>> GetMusicCategoriesAsync(Guid musicId);
    public Task<IDataResult<List<GetMovieCategoryDto>>> GetMovieCategoriesAsync(Guid movieId);
    public Task<IDataResult<List<GetMovieDto>>> GetMoviesByCategory(Guid categoryId);
    public Task<IDataResult<List<GetMusicDto>>> GetMusicsByCategory(Guid categoryId);

}