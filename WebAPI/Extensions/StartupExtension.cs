using AutoMapper;
using Core.Service;
using DTO.AutoMapper.AppRoles;
using DTO.AutoMapper.AppUsers;
using DTO.AutoMapper.Movies;
using DTO.AutoMapper.Musics;
using DTO.ValidationRules;
using Entity;
using Microsoft.AspNetCore.Identity;
using Service;
using Service.Abstracts;
using Service.Concretes;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtension
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {

            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_!?.-";
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = true;


                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.MaxFailedAccessAttempts = 3;

            }).AddPasswordValidator<PasswordValidator>().AddUserValidator<UserValidator>().
            AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        }

        public static void AddServicesWithExtensions(this IServiceCollection services)
        {
            services.AddAutoMapper(
                typeof(Program),
                typeof(MusicCategoryProfile),
                typeof(MusicProfile),
                typeof(MovieProfile),
                typeof(MovieCategoryProfile),
                typeof(UserProfile),
                typeof(RoleProfile)
            );
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IMusicCategoryService, MusicCategoryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMusicService,MusicService>();
            services.AddScoped<IMovieService,MovieService>();
            services.AddScoped<IMovieCategoryService, MovieCategoryService>();
            services.AddScoped(typeof(ITBaseService<,,,>), typeof(TBaseService<,,,>));

        }
    }
}