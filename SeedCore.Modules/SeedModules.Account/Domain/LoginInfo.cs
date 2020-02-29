using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SeedModules.Account.Domain
{
    [Table("LoginInfo")]
    public class LoginInfo
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string LoginProvider { get; set; }

        [MaxLength(512)]
        public string ProviderDisplayName { get; set; }

        [MaxLength(2048)]
        public string ProviderKey { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

    public class LoginInfoTypeConfiguration : IEntityTypeConfiguration<LoginInfo>
    {
        public void Configure(EntityTypeBuilder<LoginInfo> builder)
        {

        }
    }
}