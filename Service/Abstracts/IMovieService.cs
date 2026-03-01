using Core.Service;
using Core.Utilities.Results;
using DTO.Movies;
using Entity;

namespace Service.Abstracts;

public interface IMovieService:IDbOperationEvent<Movie,Guid,AppUser,AppDbContext>
{
    public Task<IResult> CreateMovieAsync(CreateMovieDto movieDto);
    public Task<IResult> UpdateMovieAsync(UpdateMovieDto movieDto);
    public Task<IResult> SoftDeleteMovieAsync(Guid id);
    public Task<IDataResult<List<GetMovieDto>>> GetAllMoviesAsync();
    public Task<IDataResult<GetMovieDto>> GetMovieByIdAsync(Guid id);
}
