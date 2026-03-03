using DTO.Movies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstracts;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateMovie(UpdateMovieDto movieDto)
        {
            var result = await _movieService.UpdateMovieAsync(movieDto);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(Guid id)
        {
            var result = await _movieService.SoftDeleteMovieAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var result = await _movieService.GetAllMoviesAsync();
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovieById(Guid id)
        {
            var result = await _movieService.GetMovieByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }
    }
}
