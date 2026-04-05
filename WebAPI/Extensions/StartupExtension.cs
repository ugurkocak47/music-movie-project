using AutoMapper;
using Core.Service;
using DTO.AutoMapper.AppRoles;
using DTO.AutoMapper.AppUsers;
using DTO.AutoMapper.Movies;
using DTO.AutoMapper.Musics;
using DTO.AutoMapper.Playlists;
using DTO.ValidationRules;
using Entity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service;
using Service.Abstracts;
using Service.Concretes;

namespace WebAPI.Extensions
{
    public static class StartupExtension
    {

        public static void AddDbContextWithExtension(this IServiceCollection services,string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .UseSqlServer(connectionString, t => t.MigrationsAssembly("WebAPI"))
        
            );
        }
        
        public static void AddIdentityWithExtension(this IServiceCollection services)
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

            // Cookie Configuration for Cross-Site Authentication
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS için zorunlu
                options.Cookie.SameSite = SameSiteMode.None; // Cross-site için gerekli
                options.Cookie.Name = "MusicMovieAuth"; // Cookie adı
                options.ExpireTimeSpan = TimeSpan.FromHours(2);
                options.SlidingExpiration = true;
                
                // API için redirect'leri devre dışı bırak
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

        }

        public static void AddServicesWithExtensions(this IServiceCollection services)
        {
            

            services.AddAutoMapper(
                typeof(Program),
                typeof(MovieCategoryProfile),
                typeof(MovieProfile),
                typeof(MusicProfile),
                typeof(MusicCategoryProfile),
                typeof(PlaylistProfile)
            );
            
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IMusicCategoryService, MusicCategoryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMusicService,MusicService>();
            services.AddScoped<IPlaylistService, PlaylistService>();
            services.AddScoped<IMovieService,MovieService>();
            services.AddScoped<IMovieCategoryService, MovieCategoryService>();
            services.AddScoped(typeof(ITBaseService<,,,>), typeof(TBaseService<,,,>));
            services.AddScoped<ISpotifyApiService, SpotifyApiService>();
            services.AddScoped<ITmdbApiService, TmdbApiService>();
            services.AddScoped<ICategoryLinkingService, CategoryLinkingService>();
            services.AddScoped<IRecommendationService, RecommendationService>();

        }
    }
}