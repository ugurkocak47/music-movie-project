﻿using DTO.AppUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Abstracts;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterUserDto userDto)
        {
            var result = await _userService.RegisterAsync(userDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }
            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginUserDto userDto)
        {
            var result = await _userService.LoginAsync(userDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }
            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpPut("updateuser")]
        public async Task<IActionResult> UpdateUserAsync(EditUserDto userDto)
        {
            var result = await _userService.EditUserInformationAsync(userDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }
            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpPut("changepassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangeUserPasswordDto changePasswordDto)
        {
            var result = await _userService.ChangePasswordAsync(changePasswordDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }
            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var result = await _userService.LogOutAsync();
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }
            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserAsync()
        {
            var result = await _userService.GetCurrentUserAsync();
            if (!result.Success)
            {
                return Unauthorized(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }
            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }


        [HttpPost("forgotpassword")]
        public async Task<IActionResult> SendPasswordResetLinkAsync(UserForgotPasswordDto userDto)
        {
            var result = await _userService.SendPasswordResetLinkAsync(userDto);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }
            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetUserPasswordAsync([FromQuery] Guid userId, [FromQuery] string token, [FromBody] UserResetPasswordDto userDto)
        {
            var result = await _userService.ResetUserPasswordAsync(userDto, userId, token);
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages });
            }
            return Ok(new { success = result.Success, messages = result.Messages });
        }

        [HttpGet("getallusers")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var result = await _userService.GetAllUsersAsync();
            if (!result.Success)
            {
                return BadRequest(new { success = result.Success, messages = result.Messages, data = (object?)null });
            }
            return Ok(new { success = result.Success, messages = result.Messages, data = result.Data });
        }
    }
}
