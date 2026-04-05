using AutoMapper;
using Core.Aspects.Autofac.Validation;
using Core.Service;
using Core.Utilities.Results;
using DTO.MovieCategories;
using DTO.ValidationRules;
using Entity;
using Microsoft.EntityFrameworkCore;
using Service.Abstracts;
using Service.Helpers;

namespace Service.Concretes;

public class MovieCategoryService:IMovieCategoryService
{
    private readonly AppDbContext _context;
    private readonly IMusicCategoryService _musicCategoryService;
    
    public MovieCategoryService(
        IMapper mapper, 
        ITBaseService<MovieCategory, Guid, AppUser, AppDbContext> current,
        AppDbContext context,
        IMusicCategoryService musicCategoryService)
    {
        _mapper = mapper;
        Current = current;
        _context = context;
        _musicCategoryService = musicCategoryService;
    }

    public ITBaseService<MovieCategory, Guid, AppUser, AppDbContext> Current { get; }
    private readonly IMapper _mapper;
    
    [ValidationAspect(typeof(MovieCategoryValidator))]
    public async Task<IResult> CreateMovieCategoryAsync(CreateMovieCategoryDto movieCategoryDto)
    {
        var movieCategoryMap = _mapper.Map<MovieCategory>(movieCategoryDto);
        await Current.AddAsync(movieCategoryMap);
        return new SuccessResult("Movie category added successfully.");
    }
    
    [ValidationAspect(typeof(MovieCategoryValidator))]
    public async Task<IResult> UpdateMovieCategoryAsync(UpdateMovieCategoryDto movieCategoryDto)
    {
        var category = await Current.FirstOrDefaultAsync(c => c.Id == movieCategoryDto.Id);
        if (category == null)
        {
            return new ErrorResult($"Movie category with ID {movieCategoryDto.Id} not found.");
        }

        // Map DTO properties onto the existing tracked entity
        _mapper.Map(movieCategoryDto, category);
        await Current.UpdateAsync(category);
        return new SuccessResult("Movie category updated successfully.");
    }

    public async Task<IDataResult<List<GetMovieCategoryDto>>> GetAllMovieCategoriesAsync()
    {
        var categories = await Current.GetAllListAsync();
        if (!categories.Any())
        {
            return new ErrorDataResult<List<GetMovieCategoryDto>>("No categories found.");
        }

        var categoriesMap = _mapper.Map<List<GetMovieCategoryDto>>(categories);
        return new SuccessDataResult<List<GetMovieCategoryDto>>(categoriesMap,
            "Movie categories returned successfully.");
    }

    public async Task<IDataResult<GetMovieCategoryDto>> GetMovieCategoryByIdAsync(Guid id)
    {
        var category = await _context.Set<MovieCategory>()
            .Include(mc => mc.SuggestedMusicCategories)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        
        if (category == null)
        {
            return new ErrorDataResult<GetMovieCategoryDto>($"Movie category with ID {id} not found.");
        }

        var categoryMap = _mapper.Map<GetMovieCategoryDto>(category);
        return new SuccessDataResult<GetMovieCategoryDto>(categoryMap, "Movie category returned successfully.");
    }

    public async Task<IResult> SoftDeleteMovieCategoryAsync(Guid id)
    {
        var category = await Current.FirstOrDefaultAsync(c => c.Id == id);
        if (category == null)
        {
            return new ErrorResult($"Movie category with ID {id} not found");
        }

        await Current.DeleteAsync(category);
        return new SuccessResult("Movie category deleted successfully.");
    }
    
    public async Task<IDataResult<GetMovieCategoryDto>> GetOrCreateMovieCategoryByNameAsync(string categoryName)
    {
        var normalizedName = categoryName.Trim().ToLowerInvariant();
    
        // Try to find existing category
        var existingCategory = await Current.FirstOrDefaultAsync(c => 
            c.Name.ToLower() == normalizedName);
    
        if (existingCategory != null)
        {
            var existingDto = _mapper.Map<GetMovieCategoryDto>(existingCategory);
            return new SuccessDataResult<GetMovieCategoryDto>(existingDto, "Category found.");
        }
    
        // Create new category if not found
        var newCategoryDto = new CreateMovieCategoryDto
        {
            Name = categoryName,
            Description = $"Genre: {categoryName}"
        };
    
        var createResult = await CreateMovieCategoryAsync(newCategoryDto);
        if (!createResult.Success)
        {
            return new ErrorDataResult<GetMovieCategoryDto>("Failed to create category.");
        }
    
        // Fetch the newly created category
        var newCategory = await Current.FirstOrDefaultAsync(c => c.Name.ToLower() == normalizedName);
        if (newCategory == null)
        {
            return new ErrorDataResult<GetMovieCategoryDto>("Failed to retrieve created category.");
        }
        
        //Link suggested music genres using GenreMappingHelper
        var suggestedMusicGenres = GenreMappingHelper.GetSuggestedMusicGenres(categoryName);
        await AutoLinkMusicCategoriesToMovieCategoryAsync(newCategory.Id, suggestedMusicGenres);
        
        var newDto = _mapper.Map<GetMovieCategoryDto>(newCategory);
        return new SuccessDataResult<GetMovieCategoryDto>(newDto, "Category created and linked successfully.");
    }
    
    // Automatically links music categories to a movie category.
    // Gets or creates music categories by name and adds them to the many-to-many relationship.
    private async Task AutoLinkMusicCategoriesToMovieCategoryAsync(Guid movieCategoryId, List<string> musicGenreNames)
    {
        foreach (var musicGenreName in musicGenreNames)
        {
            try
            {
                // Get or create the music category
                var musicCategoryResult = await _musicCategoryService.GetOrCreateMusicCategoryByNameAsync(musicGenreName);
                
                if (musicCategoryResult.Success && musicCategoryResult.Data != null)
                {
                    var musicCategoryId = musicCategoryResult.Data.Id;
                    
                    // Check if this relationship already exists in the database
                    var existingLink = await _context.Set<MovieCategory>()
                        .Where(mc => mc.Id == movieCategoryId)
                        .SelectMany(mc => mc.SuggestedMusicCategories)
                        .AnyAsync(mcat => mcat.Id == musicCategoryId);
                    
                    if (!existingLink)
                    {
                        // Load both entities
                        var movieCategory = await _context.Set<MovieCategory>()
                            .Include(mc => mc.SuggestedMusicCategories)
                            .FirstOrDefaultAsync(mc => mc.Id == movieCategoryId);
                            
                        var musicCategory = await _context.Set<MusicCategory>()
                            .FirstOrDefaultAsync(mc => mc.Id == musicCategoryId);
                        
                        if (movieCategory != null && musicCategory != null)
                        {
                            // Add to the collection (EF Core handles the join table)
                            movieCategory.SuggestedMusicCategories.Add(musicCategory);
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"Linked music genre '{musicGenreName}' to movie category ID {movieCategoryId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Music genre '{musicGenreName}' already linked to movie category ID {movieCategoryId}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log but don't fail the entire operation if one link fails
                Console.WriteLine($"Failed to link music genre '{musicGenreName}': {ex.Message}");
            }
        }
    }
}
