using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Project.Entities.EntityClasses.IdentityEntities
{
    public class UserProfile : BaseEntity
    {
        public UserProfile()
        {
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string ImageUrl { get; set; }

        public User User { get; set; }
        [ForeignKey(nameof(UserId))]
        public Guid UserId { get; set; }
    }


    public class UserProfileConfigurations : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
