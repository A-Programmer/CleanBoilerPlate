using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Project.Entities;
using Project.Entities.SharedEntity;

namespace Project.Core.Services.SecutiryServices
{
    public interface IJwtService
    {
        Task<AccessToken> GenerateToken(User user);
    }
}
