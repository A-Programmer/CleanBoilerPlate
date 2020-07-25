using System;
using System.Text.Json.Serialization;

namespace Project.Api.Resources.UserDtos
{
    public class ChangePasswordDto
    {
        public Guid Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
