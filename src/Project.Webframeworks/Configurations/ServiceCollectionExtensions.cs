using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Project.Common.Utilities;
using Project.Core.Services.UserServices;
using Project.Data;
using Project.Entities;
using Project.Entities.Common;
using Microsoft.OpenApi.Models;
using System.IO;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Project.Webframeworks.SwaggerConfig;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Authorization;
//using Project.Webframeworks.SwaggerConfig;

namespace Project.Webframeworks.Configurations
{
    public static class ServiceCollectionExtensions
    {

        public static void AddMinimalMvc(this IServiceCollection services)
        {
            //https://github.com/aspnet/Mvc/blob/release/2.2/src/Microsoft.AspNetCore.Mvc/MvcServiceCollectionExtensions.cs
            services.AddMvcCore(options =>
            {
                options.Filters.Add(new AuthorizeFilter());

                //Like [ValidateAntiforgeryToken] attribute but dose not validatie for GET and HEAD http method
                //You can ingore validate by using [IgnoreAntiforgeryToken] attribute
                //Use this filter when use cookie 
                //options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

                //options.UseYeKeModelBinder();
            })
            .AddApiExplorer()
            .AddAuthorization()
            .AddFormatterMappings()
            .AddDataAnnotations()
            .AddCors();
        }

        public static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    //x => x.MigrationsAssembly("Project.Data"));
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

        public static void AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1,0);
                options.ReportApiVersions = true;
            });
        }

        public static void AddSwaggerService(this IServiceCollection services)
        {
            Assert.NotNull(services, nameof(services));

            //Add services to use Example Filters in swagger
            services.AddSwaggerExamples();
            //Add services and configuration to use swagger
            services.AddSwaggerGen(options =>
            {

                var xmlDocPath = Path.Combine(AppContext.BaseDirectory, "Project.Api.xml");
                //show controller XML comments like summary
                options.IncludeXmlComments(xmlDocPath, true);

                //options.EnableAnnotations();
                //options.DescribeAllEnumsAsStrings();
                //options.DescribeAllParametersInCamelCase();
                //options.DescribeStringEnumsInCamelCase()
                //options.UseReferencedDefinitionsForEnums()
                //options.IgnoreObsoleteActions();
                //options.IgnoreObsoleteProperties();

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "IADT API",
                    //Description = "IADT API Swagger Surface",
                    //Contact = new OpenApiContact
                    //{
                    //    Name = "Kamran Sadin",
                    //    Email = "MrSadin@gmail.com",
                    //    Url = new Uri("https://www.linkedin.com/in/mesadin/")
                    //},
                    //License = new OpenApiLicense
                    //{
                    //    Name = "MIT",
                    //    Url = new Uri("https://github.com/a-Programmer/IADT")
                    //}
                });
                options.SwaggerDoc("v2", new OpenApiInfo { Version = "v2", Title = "IADT Api V2" });

                #region Filters
                //Enable to use [SwaggerRequestExample] & [SwaggerResponseExample]
                //options.ExampleFilters();

                //Adds an Upload button to endpoints which have [AddSwaggerFileUploadButton]
                //options.OperationFilter<AddFileParamTypesOperationFilter>();

                //Set summary of action if not already set
                //options.OperationFilter<ApplySummariesOperationFilter>();




                #region Add UnAuthorized to Response
                ////Enable to use [SwaggerRequestExample] & [SwaggerResponseExample]
                //options.ExampleFilters();

                ////Adds an Upload button to endpoints which have [AddSwaggerFileUploadButton]
                //options.OperationFilter<AddFileParamTypesOperationFilter>();

                ////Set summary of action if not already set
                //options.OperationFilter<ApplySummariesOperationFilter>();

                //Add 401 response and security requirements (Lock icon) to actions that need authorization
                options.OperationFilter<UnauthorizedResponsesOperationFilter>(false, "Bearer");


                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Description = @"JWT Token, Please enter token in this format: Bearer [space] token <br/> Example: 'Bearer 12345abcdef'",
                //    Name = "Authorization",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.ApiKey,
                //    Scheme = "Bearer"
                //});

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Token, Please enter token in this format: Bearer [space] token <br/> Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Scheme = "Bearer",
                    Flows = new OpenApiOAuthFlows
                    {
                         Password = new OpenApiOAuthFlow
                         {
                              TokenUrl = new Uri("https://localhost:5001/api/v1/authentication/"),
                         }

                    }
                });


                #endregion


                #region Versioning
                // Remove version parameter from all Operations
                options.OperationFilter<RemoveVersionParameters>();

                //set version "api/v{version}/[controller]" from current swagger doc verion
                options.DocumentFilter<SetVersionInPaths>();

                //Seperate and categorize end-points by doc version
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes<ApiVersionAttribute>(true)
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });
                #endregion

                //If use FluentValidation then must be use this package to show validation in swagger (MicroElements.Swashbuckle.FluentValidation)
                //options.AddFluentValidationRules();
                #endregion
            });
        }

    }
}
