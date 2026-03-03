﻿using AutoMapper;
using Core.Utilities.Results;
using Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Abstracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DTO.AppUsers;

namespace Service.Concretes
{
    public class UserService : IUserService
    {
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<AppUser> userManager, IMapper mapper, SignInManager<AppUser> signInManager, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _emailService = emailService;
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IResult> LoginAsync(LoginUserDto userDto)
        {
            var hasUser = await _userManager.FindByEmailAsync(userDto.Email!);
            if (hasUser == null)
            {
                return new ErrorResult();
            }
            // Use the existing user object directly
            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, userDto.Password, false, true);

            if (!signInResult.Succeeded)
            {
                
                return new ErrorResult();
            }
            if (signInResult.IsLockedOut)
            {
                return new ErrorResult("LockedOut");
            }
            return new SuccessResult();
        }

        public async Task<IResult> RegisterAsync(RegisterUserDto userDto)
        {
            //email doğrulama ve sms doğrulaması yapılacak

            var userMap = _mapper.Map<AppUser>(userDto);
            userMap.CreatedDate = DateTime.Now;
            var identityResult = await _userManager.CreateAsync(userMap, userDto.Password);
            if (identityResult.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);


                await _userManager.AddToRoleAsync(user, "member");

                var claim = new Claim(ClaimTypes.Role, "member");
                await _userManager.AddClaimAsync(user, claim);
                return new SuccessResult();
            }

            var errors = identityResult.Errors.Select(e => e.Description).ToList();
            return new ErrorResult(errors);
        }

        public async Task<IResult> LogOutAsync()
        {
            await _signInManager.SignOutAsync();
            return new SuccessResult();
        }

        public async Task<IResult> ChangePasswordAsync(ChangeUserPasswordDto userDto)
        {
            var currentUser = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext?.User.Identity!.Name!);

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser!, userDto.PasswordOld);
            if (!checkOldPassword)
            {
                return new ErrorResult("Old password is not correct");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(currentUser!, userDto.PasswordOld, userDto.PasswordNew);

            if (!changePasswordResult.Succeeded)
            {
                var errors = changePasswordResult.Errors.Select(e => e.Description).ToList();
                return new ErrorResult(errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser!);

            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser!, userDto.PasswordNew, true, false);

            return new SuccessResult();
        }

        public async Task<IDataResult<AppUser>> GetCurrentUserAsync()
        {
            var currentUser = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext?.User.Identity!.Name!);
            if (currentUser == null)
            {
                return new ErrorDataResult<AppUser>();
            }
            return new SuccessDataResult<AppUser>(currentUser);
        }

        public async Task<IDataResult<EditUserDto>> EditUserInformationAsync(EditUserDto userDto)
        {
            var currentUser = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext?.User.Identity!.Name!);

            currentUser.UserName = userDto.UserName;
            currentUser.Email = userDto.Email;
            currentUser.PhoneNumber = userDto.PhoneNumber;
            currentUser.BirthDate = userDto.BirthDate;
            currentUser.Gender = userDto.Gender;

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);

            if (!updateToUserResult.Succeeded)
            {
                var errors = updateToUserResult.Errors.ToList().ToString()!;
                return new ErrorDataResult<EditUserDto>(errors);
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, true);

            var updatedUser = _mapper.Map<EditUserDto>(currentUser);

            return new ErrorDataResult<EditUserDto>(updatedUser);
        }
        public async Task<IResult> SendPasswordResetLinkAsync(UserForgotPasswordDto userDto)
        {
            var user = await _userManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                return new ErrorResult("Kullanıcı bulunamadı!");
            }
            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var request = _httpContextAccessor.HttpContext?.Request;
            var passwordResetLink = $"{request?.Scheme}://{request?.Host}/User/ResetPassword?userId={user.Id}&token={passwordResetToken}";
            await _emailService.SendResetEmail(passwordResetLink!, user.Email!);
            return new SuccessResult();
        }

        public async Task<IResult> ResetUserPasswordAsync(UserResetPasswordDto userDto, Guid userId, string token)
        {
            
            if (userId == null || token == null)
            {
                return new ErrorResult("Bir hata meydana geldi.");
            }
            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user == null)
            {
                return new ErrorResult("Kullanıcı bulunamadı.");
            }


            var result = await _userManager.ResetPasswordAsync(user, token.ToString()!, userDto.Password);
            return new SuccessResult();
        }

        public async Task<IDataResult<List<ListUserDto>>> GetAllUsersAsync()
        {
           var users = await _userManager.Users.ToListAsync();
           var userList = _mapper.Map<List<ListUserDto>>(users);
            return new SuccessDataResult<List<ListUserDto>>(userList);
        }
    }
}

