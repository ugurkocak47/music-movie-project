using DTO.AppRoles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstracts;

namespace WebAPI.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("getuserroles/{id}")]
        public async Task<IActionResult> GetUserRoles(Guid id)
        {
            var result = await _roleService.GetUserRolesAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpPost("assignrole/{id}")]
        public async Task<IActionResult> AssignRoleToUser(Guid id, List<AssignRoleToUserDto> roleDto)
        {
            var result = await _roleService.AssignRoleToUserAsync(id, roleDto);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(CreateRoleDto roleDto)
        {
            var result = await _roleService.CreateRoleAsync(roleDto);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(UpdateRoleDto roleDto)
        {
            var result = await _roleService.UpdateRoleAsync(roleDto);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var result = await _roleService.GetCurrentRoleToUpdateAsync(id);
            if (!result.Success)
            {
                return BadRequest(result.Messages);
            }

            return Ok(result);
        }
    }
}
