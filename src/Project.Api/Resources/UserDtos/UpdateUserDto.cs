using System;
using System.ComponentModel.DataAnnotations;
using Project.Entities;

namespace Project.Api.Resources.UserDtos
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
    }
}
