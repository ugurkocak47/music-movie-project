using Microsoft.AspNetCore.Identity;

namespace Core.Utilities.Localization;

public class LocalizationErrorDescriber : IdentityErrorDescriber
{
    LocalizationService _localizationService;
    public override IdentityError DuplicateEmail(string email)
    {
        return new()
        {
            Code = nameof(DuplicateEmail),
            Description = _localizationService.Translate("DuplicateEmail")
        };
    }

    public override IdentityError InvalidEmail(string? email)
    {
        return new IdentityError()
        {
            Code = nameof(InvalidEmail),
            Description = _localizationService.Translate("InvalidEmail")
        };
    }

    public override IdentityError InvalidRoleName(string? role)
    {
        return new IdentityError()
        {
            Code = nameof(InvalidRoleName),
            Description = _localizationService.Translate("InvalidRoleName")
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = _localizationService.Translate("PasswordRequiresDigit")
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = _localizationService.Translate("PasswordRequiresLower")
        };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = _localizationService.Translate("PasswordRequiresNonAlphanumeric")
        };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new()
        {
            Code = nameof(DuplicateUserName),
            Description = _localizationService.Translate("DuplicateUserName")
        };
    }

    public override IdentityError UserAlreadyHasPassword()
    {
        return new()
        {
            Code = nameof(UserAlreadyHasPassword),
            Description = _localizationService.Translate("UserAlreadyHasPassword")
        };
    }

    override public IdentityError PasswordMismatch()
    {
        return new()
        {
            Code = nameof(PasswordMismatch),
            Description = _localizationService.Translate("PasswordMismatch")
        };
    }

    override public IdentityError PasswordTooShort(int length)
    {
        return new()
        {
            Code = nameof(PasswordTooShort),
            Description = _localizationService.Translate("PasswordTooShort")
        };
    }

    public override IdentityError UserAlreadyInRole(string role)
    {
        return new()
        {
            Code = nameof(UserAlreadyInRole),
            Description = _localizationService.Translate("UserAlreadyInRole", role)
        };
    }

    public override IdentityError UserNotInRole(string role)
    {
        return new ()
        {
            Code = nameof(UserNotInRole),
            Description = _localizationService.Translate("UserNotInRole", role)
        };
    }

    public override IdentityError UserLockoutNotEnabled()
    {
        return new()
        {
            Code = nameof(UserLockoutNotEnabled),
            Description = _localizationService.Translate("UserLockoutNotEnabled")
        };
    }
}
