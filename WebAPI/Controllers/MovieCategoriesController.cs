using DTO.MovieCategories;
using DTO.Movies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstracts;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieCategoriesController : ControllerBase
    {
        private readonly IMovieCategoryService _movieCategoryService;

        public MovieCategoriesController(IMovieCategoryService movieCategoryService)
        {
            _movieCategoryService = movieCategoryService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(CreateMovieCategoryDto movieDto)
        {
            var result = await _movieCategoryService.CreateMovieCategoryAsync(movieDto);
            if (!result.Success)
            {
               return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMovie(UpdateMovieCategoryDto movieDto)
        {
            var result = await _movieCategoryService.UpdateMovieCategoryAsync(movieDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var result = await _movieCategoryService.SoftDeleteMovieCategoryAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var result = await _movieCategoryService.GetAllMovieCategoriesAsync();
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }

            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var result = await _movieCategoryService.GetMovieCategoryByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }

            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }
        
    }
}
