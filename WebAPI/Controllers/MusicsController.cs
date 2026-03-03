using DTO.Musics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstracts;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicsController : ControllerBase
    {
        private readonly IMusicService _musicService;

        public MusicsController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMusic(CreateMusicDto musicDto)
        {
            var result = await _musicService.CreateMusicAsync(musicDto);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMusic(UpdateMusicDto musicDto)
        {
            var result = await _musicService.UpdateMusicAsync(musicDto);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMusic(Guid id)
        {
            var result = await _musicService.SoftDeleteMusicAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMusics()
        {
            var result = await _musicService.GetAllMusicsAsync();
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMusicById(Guid id)
        {
            var result = await _musicService.GetMusicByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }
    }
}
