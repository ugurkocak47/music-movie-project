using Core.Utilities.Results;
using DTO.Movies;

namespace Service.Abstracts;

public interface ITmdbApiService
{
    Task<IDataResult<CreateMovieDto>> FetchMovieFromTmdbAsync(string movieTitle);
}