using System;
using System.Text.Json.Serialization;

namespace Project.Api.Resources.UserDtos
{
    public class UserLoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
