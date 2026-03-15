using DTO.MusicCategories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstracts;

namespace WebAPI.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicCategoriesController : ControllerBase
    {
        private readonly IMusicCategoryService _musicCategoryService;

        public MusicCategoriesController(IMusicCategoryService musicCategoryService)
        {
            _musicCategoryService = musicCategoryService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMusicCategory(CreateMusicCategoryDto musicCategoryDto)
        {
            var result = await _musicCategoryService.CreateMusicCategoryAsync(musicCategoryDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMusicCategory(UpdateMusicCategoryDto musicCategoryDto)
        {
            var result = await _musicCategoryService.UpdateMusicCategoryAsync(musicCategoryDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMusicCategory(Guid id)
        {
            var result = await _musicCategoryService.SoftDeleteMusicCategoryAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }

            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMusicCategories()
        {
            var result = await _musicCategoryService.GetAllMusicCategoriesAsync();
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }

            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMusicCategoryById(Guid id)
        {
            var result = await _musicCategoryService.GetMusicCategoryByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }

            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }
    }
}
