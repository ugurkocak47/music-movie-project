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

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMusicSuggestionsFromMovie(string movieTitle)
        {
            var result = await _recommendationService.GetMusicForMovieAsync(movieTitle);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }
    }
}
