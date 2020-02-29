using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrchardCore.Security;

namespace SeedModules.Account.Domain
{
    [Table("Role")]
    public class RoleInfo : IRole
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50), Required]
        public string RoleName { get; set; }

        [StringLength(50)]
        public string NormalizedRolename { get; set; }

        [StringLength(50)]
        public string DisplayName { get; set; }

        public virtual List<RoleInfoClaim> RoleClaims { get; } = new List<RoleInfoClaim>();

        public virtual List<UserRole> Users { get; set; } = new List<UserRole>();

        public RoleInfo() { }

        public RoleInfo(string rolename)
        {
            RoleName = rolename;
        }
    }

    public class RoleTypeConfiguration : IEntityTypeConfiguration<RoleInfo>
    {
        public void Configure(EntityTypeBuilder<RoleInfo> builder)
        {
            builder.HasAlternateKey(e => e.RoleName);
        }
    }
}