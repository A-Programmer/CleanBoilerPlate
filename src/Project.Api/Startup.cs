using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Project.Common.Utilities;
using Project.Core;
using Project.Core.Services.SecutiryServices;
using Project.Core.Services.TestServices;
using Project.Core.Services.UserServices;
using Project.Data;
using Project.Data.Extensions;
using Project.Entities;
using Project.Entities.Common;
using Project.Services.JwtServices;
using Project.Services.TestServices;
using Project.Services.UserServices;
using Project.Webframeworks.Configurations;
using Project.Webframeworks.Middlewares;

namespace Project.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly PublicSettings _settings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _settings = configuration.GetSection("PublicSettings").Get<PublicSettings>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<PublicSettings>(Configuration.GetSection("PublicSettings"));

            services.AddDbContext(Configuration);

            services.AddCustomIdentity(_settings.CustomIdentityOptions);

            services.AddJwtAuth(_settings.JwtOptions);

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IJwtService, JwtService>();

            //Delete later
            services.AddTransient<ITestService, TestService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddControllers().
                AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCasePropertyNamingPolicy();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            app.UseExceptionLogger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.MigrateDatabase();

            if(env.IsDevelopment())
                DataSeederExtension.SeedUserRoles(userManager, roleManager);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
