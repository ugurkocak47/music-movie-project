using AutoMapper;
using Core.Aspects.Autofac.Validation;
using Core.Helpers;
using Core.Service;
using Core.Utilities.Results;
using DTO.Movies;
using DTO.ValidationRules;
using Entity;
using Microsoft.EntityFrameworkCore;
using Service.Abstracts;

namespace Service.Concretes;

public class MovieService:IMovieService
{
    public ITBaseService<Movie, Guid, AppUser, AppDbContext> Current { get; }
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;
    
    public MovieService(
        ITBaseService<Movie, Guid, AppUser, AppDbContext> current, 
        IMapper mapper,
        AppDbContext context)
    {
        Current = current;
        _mapper = mapper;
        _context = context;
    }
    
    [ValidationAspect(typeof(MovieValidator))]
    public async Task<IResult> CreateMovieAsync(CreateMovieDto movieDto)
    {
        var doesMovieExist = await Current.FirstOrDefaultAsync(m => m.NormalizedTitle == movieDto.NormalizedTitle);
        if (doesMovieExist != null)
        {
            return new ErrorResult("Movie already exists");
        }
        var movieMap = _mapper.Map<Movie>(movieDto);
        movieMap.PosterPath = "https://image.tmdb.org/t/p/w500" + movieMap.PosterPath;
        await Current.AddAsync(movieMap);
        
        // Link categories if provided
        if (movieDto.Categories != null && movieDto.Categories.Any())
        {
            // Find the saved movie by TmdbId to get its database ID
            var savedMovie = await Current.FirstOrDefaultAsync(m => m.TmdbId == movieMap.TmdbId);
            if (savedMovie != null)
            {
                foreach (var category in movieDto.Categories)
                {
                    // Create MovieCategoryLink directly
                    var link = new MovieCategoryLink
                    {
                        MovieId = savedMovie.Id,
                        CategoryId = category.Id
                    };
                    await _context.Set<MovieCategoryLink>().AddAsync(link);
                }
                await _context.SaveChangesAsync();
            }
        }
        
        return new SuccessResult("Movie added successfully.");
    }
    
    [ValidationAspect(typeof(MovieValidator))]
    public async Task<IResult> UpdateMovieAsync(UpdateMovieDto movieDto)
    {
        var movie = await Current.FirstOrDefaultAsync(m => m.Id == movieDto.Id);
        if (movie == null)
        {
            return new ErrorResult($"Movie with ID {movieDto.Id} not found.");
        }

        // Map DTO properties onto the existing tracked entity
        _mapper.Map(movieDto, movie);
        await Current.UpdateAsync(movie);
        return new SuccessResult("Movie updated successfully.");
    }

    public async Task<IResult> SoftDeleteMovieAsync(Guid id)
    {
        var movie = await Current.FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return new ErrorResult($"Movie with ID {id} not found.");
        }

        await Current.DeleteAsync(movie);
        return new SuccessResult("Movie deleted successfully.");
    }

    public async Task<IDataResult<List<GetMovieDto>>> GetAllMoviesAsync()
    {
        var movies = await Current.GetAllListAsync();
        if (!movies.Any())
        {
            return new ErrorDataResult<List<GetMovieDto>>("No movies found.");
        }

        var moviesMap = _mapper.Map<List<GetMovieDto>>(movies);
        return new SuccessDataResult<List<GetMovieDto>>(moviesMap, "Movies returned successfully.");
    }

    public async Task<IDataResult<GetMovieDto>> GetMovieByIdAsync(Guid id)
    {
        var movie = await Current.FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
        {
            return new ErrorDataResult<GetMovieDto>($"Movie with ID {id} not found.");
        }

        var movieMap = _mapper.Map<GetMovieDto>(movie);
        return new SuccessDataResult<GetMovieDto>(movieMap, "Movie returned successfully.");
    }

    public async Task<IDataResult<GetMovieDto>> GetMovieByTmdbIdAsync(int tmdbId)
    {
        var movie = await Current.FirstOrDefaultAsync(m => m.TmdbId == tmdbId);
        if (movie == null)
        {
            return new ErrorDataResult<GetMovieDto>($"Movie with TMDB Id {tmdbId.ToString()}");
        }

        var movieMap = _mapper.Map<GetMovieDto>(movie);
        return new SuccessDataResult<GetMovieDto>(movieMap, "Movie returned successfully.");
    }

    public async Task<IDataResult<GetMovieDto>> GetMovieByTitleAsync(string title)
    {
        var normalizedTitle = StringHelper.NormalizeTurkish(title);
        var movie = await Current.FirstOrDefaultAsync(m => m.NormalizedTitle == normalizedTitle);
        if (movie == null)
        {
            return new ErrorDataResult<GetMovieDto>($"Movie with title {title} not found.");
        }

        var movieMap = _mapper.Map<GetMovieDto>(movie);
        return new SuccessDataResult<GetMovieDto>(movieMap, "Movie returned successfully.");
    }
}
