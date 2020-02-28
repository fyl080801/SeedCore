using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SeedCore.Data.Migrations
{
    public class MigrationRecordTypeConfiguration : IEntityTypeConfiguration<MigrationRecord>
    {
        public void Configure(EntityTypeBuilder<MigrationRecord> builder)
        {
            builder.ToTable("$MigrationRecord")
                .HasKey(e => e.Id);
        }
    }
}