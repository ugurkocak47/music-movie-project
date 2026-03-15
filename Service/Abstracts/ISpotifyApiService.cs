using Core.Utilities.Results;
using DTO.Movies;
using DTO.MusicCategories;
using DTO.Musics;
using Entity;

namespace Service.Abstracts;

public interface ISpotifyApiService
{
    Task<IDataResult<List<CreateMusicDto>>> FetchMusicByGenreAsync(string genreName, int limit = 5);
    Task<IDataResult<List<CreateMusicCategoryDto>>> GetAllNewMusicGenresAsync();
}