using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Project.Common.Utilities;
using Project.Core.Services.UserServices;
using Project.Data;
using Project.Entities;
using Project.Entities.Common;

namespace Project.Webframeworks.Configurations
{
    public static class ServiceCollectionExtensions
    {

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                //     x => x.MigrationsAssembly("Project.Data"));
                    options.UseNpgsql(configuration.GetConnectionString("PostgresConnection"),
                    x => x.MigrationsAssembly("Project.Data"));
            });
        }

        public static void AddJwtAuth(this IServiceCollection services, JwtOptions settings)
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(settings.SecretKey);
                var secretKey2 = Encoding.UTF8.GetBytes(settings.SecretKey2);

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    RequireSignedTokens = true,

                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = settings.Audience,

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    TokenDecryptionKey = new SymmetricSecurityKey(secretKey2)
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }

                        if (context.Exception != null)
                            throw new AuthenticationException("Failed authentication");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity.Claims?.Any() != true)
                            context.Fail("No claims has been found.");

                        var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        if (!securityStamp.HasValue())
                            throw new AuthenticationException("Token has no securitystamp.");

                        var userIdString = claimsIdentity.GetUserId();
                        var userId = Guid.Parse(userIdString);
                        var user = await userService.GetUser(userId);


                        //Validate by securitystamp
                        var validUser = await signInManager.ValidateSecurityStampAsync(context.Principal);

                        if (validUser == null)
                            throw new AuthenticationException("Securitystamp is not valid.");

                        //Custom validation like IsActive
                        if (!user.IsActive)
                            context.Fail("The user is not active.");

                        await userService.UpdateLoginDate(user);
                    },
                    OnChallenge = context =>
                    {
                        if (context.AuthenticateFailure != null)
                            throw new AuthenticationException(context.AuthenticateFailure.Message, context.AuthenticateFailure.InnerException);
                        throw new UnauthorizedAccessException("OnChallenge exception" + context.Error + "--" + context.ErrorDescription);

                    }
                };
            });
        }

        public static void AddCustomIdentity(this IServiceCollection services, CustomIdentityOptions identityOptions)
        {
            services.AddIdentity<User, Role>(options =>
            {
                //options.Password.RequireDigit = identityOptions.PasswordOptions.RequireDigit;
                options.Password.RequiredLength = identityOptions.PasswordOptions.RequiredLength;
                //options.Password.RequiredUniqueChars = identityOptions.PasswordOptions.RequiredUniqueChars;
                //options.Password.RequireLowercase = identityOptions.PasswordOptions.RequireLowercase;
                //options.Password.RequireNonAlphanumeric = identityOptions.PasswordOptions.RequireNonAlphanumeric;
                //options.Password.RequireUppercase = identityOptions.PasswordOptions.RequireUppercase;


                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;

                options.User.RequireUniqueEmail = identityOptions.UserOptions.RequireUniqueEmail;
                //options.User.AllowedUserNameCharacters = identityOptions.UserOptions.AllowedUserNameCharacters;

                ////SignIn options
                //options.SignIn.RequireConfirmedAccount = identityOptions.SigninOptions.RequireConfirmedAccount;
                //options.SignIn.RequireConfirmedEmail = identityOptions.SigninOptions.RequireConfirmedEmail;
                //options.SignIn.RequireConfirmedPhoneNumber = identityOptions.SigninOptions.RequireConfirmedPhoneNumber;

                ////Lockout options
                //options.Lockout.AllowedForNewUsers = identityOptions.LockoutOptions.AllowedForNewUsers;
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(identityOptions.LockoutOptions.DefaultLockoutMinutes);
                //options.Lockout.MaxFailedAccessAttempts = identityOptions.LockoutOptions.MaxFailedAccessAttempts;

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

    }
}
