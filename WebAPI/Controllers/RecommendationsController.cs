using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstracts;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IMovieService _movieService;

        public RecommendationsController(IRecommendationService recommendationService, IMovieService movieService)
        {
            _recommendationService = recommendationService;
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMusicSuggestionsFromMovie(string movieTitle)
        {
            var result = await _recommendationService.GetMusicForMovieAsync(movieTitle);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            var movie = await _movieService.GetMovieByTitleAsync(movieTitle);
            return Ok( new {result = result,movie = movie.Data});
        }
    }
}
