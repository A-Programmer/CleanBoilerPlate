using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Project.Api.Resources.UserDtos
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public DateTimeOffset RegisteredAt { get; set; }
        public DateTimeOffset LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string ImageUrl { get; set; }
    }
}
