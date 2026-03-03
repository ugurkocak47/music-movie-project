
using Core.Utilities.Results;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.AppUsers;

namespace Service.Abstracts
{
    public interface IUserService
    {
        public Task<IResult> LoginAsync(LoginUserDto userDto);

        public Task<IResult> RegisterAsync(RegisterUserDto userDto);

        public Task<IResult> LogOutAsync();

        public Task<IResult> ChangePasswordAsync(ChangeUserPasswordDto userDto);
        public Task<IDataResult<List<ListUserDto>>> GetAllUsersAsync();

        public Task<IDataResult<AppUser>> GetCurrentUserAsync();

        public Task<IDataResult<EditUserDto>> EditUserInformationAsync(EditUserDto userDto);

        public Task<IResult> SendPasswordResetLinkAsync(UserForgotPasswordDto userDto);
        public Task<IResult> ResetUserPasswordAsync(UserResetPasswordDto userDto, Guid userId, string token);
    }
}
