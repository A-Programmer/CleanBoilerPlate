using System;
using System.Text.Json.Serialization;

namespace Project.Api.Resources.UserDtos
{
    public class ChangePasswordWithTokenDto
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
