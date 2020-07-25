using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Project.Entities;

namespace Project.Core.Services.SecutiryServices
{
    public interface IJwtService
    {
        Task<string> GenerateToken(User user);
    }
}
