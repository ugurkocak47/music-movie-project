using DTO.Musics;
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
    public class MusicsController : ControllerBase
    {
        private readonly IMusicService _musicService;
        private readonly ICategoryLinkingService _linkingService;
        private readonly IMusicCategoryService _categoryService;

        public MusicsController(IMusicService musicService, ICategoryLinkingService linkingService, IMusicCategoryService categoryService)
        {
            _musicService = musicService;
            _linkingService = linkingService;
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMusic(CreateMusicDto musicDto)
        {
            var result = await _musicService.CreateMusicAsync(musicDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMusic(UpdateMusicDto musicDto)
        {
            var result = await _musicService.UpdateMusicAsync(musicDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMusic(Guid id)
        {
            var result = await _musicService.SoftDeleteMusicAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMusics()
        {
            var result = await _musicService.GetAllMusicsAsync();
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }

            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMusicById(Guid id)
        {
            var result = await _musicService.GetMusicByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }

            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }

        [Authorize(Roles = "member")]
        [HttpGet("getmusicsbycategory/{id}")]
        public async Task<IActionResult> GetMusicsByCategory(Guid id)
        {
            var result = await _linkingService.GetMusicsByCategory(id);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            var categoryName = await _categoryService.GetMusicCategoryByIdAsync(id);
            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data, category=categoryName.Data.Name });
        }
    }
}
