using System;
using System.Text.Json.Serialization;

namespace Project.Api.Resources.UserDtos
{
    public class ForgotPasswordDto
    {
        public string UserName { get; set; }
    }
}
