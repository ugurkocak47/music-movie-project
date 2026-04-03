using AutoMapper;
using Core.Service;
using Core.Utilities.Results;
using DTO.MovieCategories;
using DTO.MusicCategories;
using Entity;
using Service.Abstracts;

namespace Service.Concretes;

public class CategoryLinkingService:ICategoryLinkingService
{
    private readonly ITBaseService<MovieCategoryLink, Guid, AppUser, AppDbContext> _movieLinker;
    private readonly ITBaseService<MusicCategoryLink, Guid, AppUser, AppDbContext> _musicLinker;

    private readonly IMovieService _movieService;
    private readonly IMusicService _musicService;

    private readonly IMusicCategoryService _musicCategoryService;
    private readonly IMovieCategoryService _movieCategoryService;
    
    private readonly IMapper _mapper;
    
    public CategoryLinkingService(ITBaseService<MovieCategoryLink, Guid, AppUser, AppDbContext> movieLinker, ITBaseService<MusicCategoryLink, Guid, AppUser, AppDbContext> musicLinker, IMapper mapper, IMovieService movieService, IMusicService musicService, IMovieCategoryService movieCategoryService, IMusicCategoryService musicCategoryService)
    {
        _movieLinker = movieLinker;
        _musicLinker = musicLinker;
        _mapper = mapper;
        _movieService = movieService;
        _musicService = musicService;
        _movieCategoryService = movieCategoryService;
        _musicCategoryService = musicCategoryService;
    }

    ITBaseService<MovieCategoryLink, Guid, AppUser, AppDbContext> IDbOperationEvent<MovieCategoryLink, Guid, AppUser, AppDbContext>.Current => _movieLinker;

    ITBaseService<MusicCategoryLink, Guid, AppUser, AppDbContext> IDbOperationEvent<MusicCategoryLink, Guid, AppUser, AppDbContext>.Current => _musicLinker;

    public async Task<IResult> LinkMusicToCategoryAsync(string musicSpotifyId, Guid categoryId)
    {
        var result = await _musicService.GetMusicBySpotifyIdAsync(musicSpotifyId);
        if (!result.Success)
        {
            return new ErrorResult("Music linking error: " + result.Messages[0]);
        }
        var link = new MusicCategoryLink()
        {
            CategoryId = categoryId,
            MusicId = result.Data.Id
        };
        await _musicLinker.AddAsync(link);
        return new SuccessResult("Link created successfully.");
    }

    public async Task<IResult> LinkMovieToCategoryAsync(int movieTmdbId, Guid categoryId)
    {
        var result = await _movieService.GetMovieByTmdbIdAsync(movieTmdbId);
        if (!result.Success)
        {
            return new ErrorResult("Movie linking error: " + result.Messages[0]);
        }

        var link = new MovieCategoryLink()
        {
            CategoryId = categoryId,
            MovieId = result.Data.Id
        };
        await _movieLinker.AddAsync(link);
        return new SuccessResult("Link created successfully.");

    }

    public async Task<IDataResult<List<GetMusicCategoryDto>>> GetMusicCategoriesAsync(Guid musicId)
    {
        var musicAvail = await _musicService.GetMusicByIdAsync(musicId);
        if (!musicAvail.Success)
        {
            return new ErrorDataResult<List<GetMusicCategoryDto>>($"Music with ID {musicId.ToString()} not found.");
        }

        var categoryLinks = await _musicLinker.GetAllListAsync(c => c.MusicId == musicId);
        List<GetMusicCategoryDto> categories = new List<GetMusicCategoryDto>(); 
        foreach (var link in categoryLinks)
        {
            var category = await _musicCategoryService.GetMusicCategoryByIdAsync(link.CategoryId);
            if (!category.Success)
            {
                return new ErrorDataResult<List<GetMusicCategoryDto>>("Category not found.");
            }
            categories.Add(category.Data);
        }

        return new SuccessDataResult<List<GetMusicCategoryDto>>(categories, "Categories returned successfully.");
    }

    public async Task<IDataResult<List<GetMovieCategoryDto>>> GetMovieCategoriesAsync(Guid movieId)
    {
        var movieAvail = await _movieService.GetMovieByIdAsync(movieId);
        if (!movieAvail.Success)
        {
            return new ErrorDataResult<List<GetMovieCategoryDto>>($"Movie with ID {movieId.ToString()} not found.");
        }

        var categoryLinks = await _movieLinker.GetAllListAsync(c => c.MovieId == movieId);
        List<GetMovieCategoryDto> categories = new List<GetMovieCategoryDto>();

        foreach (var link in categoryLinks)
        {
            var category = await _movieCategoryService.GetMovieCategoryByIdAsync(link.CategoryId);
            if (!category.Success)
            {
                return new ErrorDataResult<List<GetMovieCategoryDto>>("Category not found.");
            }
            categories.Add(category.Data);
        }

        return new SuccessDataResult<List<GetMovieCategoryDto>>(categories, "Categories returned successfully.");
    }
}