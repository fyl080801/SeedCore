using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrchardCore.Users;

namespace SeedModules.Account.Domain
{
    [Table("User")]
    public class User : IUser
    {
        [Key]
        public int Id { get; set; }

        [StringLength(20), Required]
        public string UserName { get; set; }

        [StringLength(20)]
        public string NormalizedUsername { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(200)]
        public string NormalizedEmail { get; set; }

        [StringLength(500)]
        public string PasswordHash { get; set; }

        [StringLength(500)]
        public string SecurityStamp { get; set; }

        public bool EmailConfirmed { get; set; }

        [StringLength(25)]
        public string FirstName { get; set; }

        [StringLength(25)]
        public string LastName { get; set; }

        [StringLength(512)]
        public string ResetToken { get; set; }

        public virtual List<UserRole> Roles { get; set; } = new List<UserRole>();

        public virtual List<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

        public virtual List<LoginInfo> UserLogins { get; set; } = new List<LoginInfo>();

        public override string ToString()
        {
            return UserName;
        }
    }

    public class UserTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasAlternateKey(e => e.UserName);
        }
    }
}