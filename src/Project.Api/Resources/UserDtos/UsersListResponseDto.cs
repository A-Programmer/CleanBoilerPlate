using System;
using System.Text.Json.Serialization;

namespace Project.Api.Resources.UserDtos
{
    public class UsersListResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset RegisteredAt { get; set; }
    }
}
