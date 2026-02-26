using Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Service.Helpers
{
    public class ClaimsHelper
    {
        public static async Task AddClaimIfExist(UserManager<AppUser> userManager, AppUser user, string claimType, string claimValue)
        {
            if (!string.IsNullOrEmpty(claimValue))
            {
                await userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
            }
        }
    }
}
