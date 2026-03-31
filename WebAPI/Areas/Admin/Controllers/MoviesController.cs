using DTO.Movies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstracts;

namespace WebAPI.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }
        
        [HttpPost]
        public async Task<IActionResult> AddMovie(CreateMovieDto movieDto)
        {
            var result = await _movieService.CreateMovieAsync(movieDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }


        [HttpPut]
        public async Task<IActionResult> UpdateMovie(UpdateMovieDto movieDto)
        {
            var result = await _movieService.UpdateMovieAsync(movieDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var result = await _movieService.SoftDeleteMovieAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var result = await _movieService.GetAllMoviesAsync();
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }

            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var result = await _movieService.GetMovieByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }

            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }
    }
}
