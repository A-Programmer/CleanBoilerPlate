using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Entities;

namespace Project.Data.Extensions
{
    public static class DataSeederExtension
    {
        public static void SeedUserRoles(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var adminRole = new Role()
            {
                Name = "admin",
                NormalizedName = "ADMIN",
                Description = "This is admins role"
            };
            var userRole = new Role()
            {
                Name = "user",
                NormalizedName = "USER",
                Description = "This is users role"
            };

            if (roleManager.FindByNameAsync("admin").Result == null)
            {
                roleManager.CreateAsync(adminRole).Wait();
            }
            if (roleManager.FindByNameAsync("user").Result == null)
            {
                roleManager.CreateAsync(userRole).Wait();
            }




            var admin = new User()
            {
                UserName = "admin",
                Email = "admin@admin.com",
                NormalizedUserName = "ADMIN",
                IsActive = true,
                LastLoginDate = DateTimeOffset.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var user = new User()
            {
                UserName = "user",
                Email = "user@user.com",
                NormalizedUserName = "USER",
                IsActive = true,
                LastLoginDate = DateTimeOffset.UtcNow,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            if (userManager.FindByEmailAsync("admin@admin.com").Result == null)
            {
                IdentityResult result = userManager.CreateAsync(admin, "123456").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, "admin").Wait();
                }
            }
            if (userManager.FindByEmailAsync("user@user.com").Result == null)
            {
                IdentityResult result = userManager.CreateAsync(user, "123456").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "user").Wait();
                }
            }
        }
    }
}
