using AutoMapper;
using Core.Aspects.Autofac.Validation;
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
        var movieMap = _mapper.Map<Movie>(movieDto);
        
        // Handle existing MovieCategory entities to prevent tracking conflicts
        if (movieMap.Categories != null && movieMap.Categories.Any())
        {
            var categoryList = movieMap.Categories.ToList();
            movieMap.Categories.Clear();
            
            foreach (var category in categoryList)
            {
                // Check if this category is already being tracked
                var trackedCategory = _context.ChangeTracker.Entries<MovieCategory>()
                    .FirstOrDefault(e => e.Entity.Id == category.Id);
                
                if (trackedCategory != null)
                {
                    // Use the already-tracked entity
                    movieMap.Categories.Add(trackedCategory.Entity);
                }
                else
                {
                    // Attach as unchanged (existing entity)
                    _context.Entry(category).State = EntityState.Unchanged;
                    movieMap.Categories.Add(category);
                }
            }
        }
        
        await Current.AddAsync(movieMap);
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

    public async Task<IDataResult<GetMovieDto>> GetMovieByTitleAsync(string title)
    {
        var movie = await Current.FirstOrDefaultAsync(m=>m.NormalizedTitle == title.ToUpper());
        if (movie == null)
        {
            return new ErrorDataResult<GetMovieDto>($"Movie with title {title} not found.");
        }

        var movieMap = _mapper.Map<GetMovieDto>(movie);
        return new SuccessDataResult<GetMovieDto>(movieMap, "Movie returned successfully.");
    }
}
