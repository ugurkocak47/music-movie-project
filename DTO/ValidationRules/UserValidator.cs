using Core.Utilities.Localization;
using Entity;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.ValidationRules
{
    public class UserValidator:AbstractValidator<AppUser>, IUserValidator<AppUser>
    {
        //private readonly LocalizationService _localization;
        public UserValidator()
        {


            RuleFor(x => x.Email).NotEmpty().WithMessage("EmailRequired");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserNameRequired");
            RuleFor(x => x.Gender).NotEmpty().WithMessage("GenderRequired");
            RuleFor(x => x.Name).NotEmpty().WithMessage("NameRequired");
            RuleFor(x => x.Surname).NotEmpty().WithMessage("SurnameRequired");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("PhoneNumberRequired");
        }
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();
            var isDigit = int.TryParse(user.UserName[0].ToString()!, out _);

            if (isDigit)
            {
                errors.Add(new()
                {
                    Code = "UserNameContainFirstLetterDigit",
                    Description = "UserNameContainFirstLetterDigit"
                });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
