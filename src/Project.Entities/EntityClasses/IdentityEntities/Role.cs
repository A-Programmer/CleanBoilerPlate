using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Project.Entities.EntityClasses.IdentityEntities
{
    public class Role : IdentityRole<Guid>, IEntity
    {
        public Role()
        {
        }
        
        public string Description { get; set; }
        
    }


    public class RoleConfigurations : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
