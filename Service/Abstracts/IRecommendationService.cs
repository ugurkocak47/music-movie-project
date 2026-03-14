using Core.Utilities.Results;
using DTO.Musics;

namespace Service.Abstracts;

public interface IRecommendationService
{
    Task<IDataResult<List<GetMusicDto>>> GetMusicForMovieAsync(string movieTitle);
}