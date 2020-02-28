using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SeedModules.Admin.Domain
{
    [Table("TestTable")]
    public class TestTable
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public string Remark { get; set; }
    }

    public class TestTableConfiguration : IEntityTypeConfiguration<TestTable>
    {
        public void Configure(EntityTypeBuilder<TestTable> builder)
        {

        }
    }
}