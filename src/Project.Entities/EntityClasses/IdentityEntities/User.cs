using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Project.Entities.EntityClasses.IdentityEntities
{
    public class User : IdentityUser<Guid>, IEntity
    {
        public User()
        {
        }
        public DateTimeOffset RegisteredAt { get; set; }
        public DateTimeOffset LastLoginDate { get; set; }
        public bool IsActive { get; set; }

        public UserProfile Profile { get; set; }
    }

    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Profile)
                .WithOne(y => y.User)
                .HasForeignKey<UserProfile>(y => y.UserId);
        }
    }
}
