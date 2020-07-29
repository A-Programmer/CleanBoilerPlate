using System;
using Microsoft.AspNetCore.Identity;

namespace Project.Entities
{
    public class User : IdentityUser<Guid>, IEntity
    {
        public User()
        {
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public DateTimeOffset RegisteredAt { get; set; }
        public DateTimeOffset LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public string ImageUrl { get; set; }
        public int VerificationCode { get; set; }
    }
}
