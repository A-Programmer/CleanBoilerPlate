using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Project.Api.Resources.UserDtos
{
    public class UserRegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
