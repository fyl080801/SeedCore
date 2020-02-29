using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SeedModules.Account.Domain
{
    [Table("RoleClaim")]
    public class RoleInfoClaim
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string ClaimType { get; set; }

        [StringLength(500)]
        public string ClaimValue { get; set; }

        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual RoleInfo Role { get; set; }

        public Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }
    }

    public class RoleClaimTypeConfiguration : IEntityTypeConfiguration<RoleInfoClaim>
    {
        public void Configure(EntityTypeBuilder<RoleInfoClaim> builder)
        {

        }
    }
}
