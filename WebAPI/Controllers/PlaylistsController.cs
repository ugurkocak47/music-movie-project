using DTO.Playlists;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstracts;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPlaylist(CreatePlaylistDto playlistDto)
        {
            var result = await _playlistService.CreatePlaylistAsync(playlistDto);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePlaylist(UpdatePlaylistDto playlistDto)
        {
            var result = await _playlistService.UpdatePlaylistAsync(playlistDto);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist(Guid id)
        {
            var result = await _playlistService.DeletePlaylistAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlaylists(bool getPrivate)
        {
            var result = await _playlistService.GetAllPlaylistsAsync(getPrivate);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpGet("getbyuser/{id}")]
        public async Task<IActionResult> GetPlaylistsByUserId(Guid userId, bool getPrivate)
        {
            var result = await _playlistService.GetPlaylistsByUserId(userId, getPrivate);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpGet("getbymovie/{id}")]
        public async Task<IActionResult> GetPlaylistsByMovieId(Guid movieId, bool getPrivate)
        {
            var result = await _playlistService.GetPlaylistsByMovieId(movieId, getPrivate);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result.Messages);
        }
        
    }
}
