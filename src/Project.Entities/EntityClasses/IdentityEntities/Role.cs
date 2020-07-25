using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Project.Entities
{
    public class Role : IdentityRole<Guid>, IEntity
    {
        public Role()
        {
        }
        
        public string Description { get; set; }
        
    }
}
