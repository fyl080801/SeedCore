using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrchardCore.Environment.Shell;

namespace SeedCore.Data
{
    public class ModuleDbContext : DbContext, IDocumentDbContext
    {
        readonly IEnumerable<object> _entityConfigurations;
        readonly ShellSettings _settings;

        public DbContext Context => this;

        public DbSet<Document> Document { get; set; }

        public DbSet<MigrationRecord> Migrations { get; set; }

        public ModuleDbContext(
            DbContextOptions options,
            ShellSettings settings,
            params object[] entityConfigurations)
            : base(options)
        {
            _entityConfigurations = entityConfigurations;
            _settings = settings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.ApplyConfiguration(new MigrationTypeConfiguration());

            foreach (var configuration in _entityConfigurations)
            {
                modelBuilder.ApplyConfiguration((dynamic)configuration);
            }

            modelBuilder.Model
                .GetEntityTypes()
                .ToList()
                .ForEach(e => modelBuilder.Entity(e.Name).ToTable($"{_settings["TablePrefix"]}_{e.GetTableName()}"));

            base.OnModelCreating(modelBuilder);
        }

        public override DbSet<TEntity> Set<TEntity>()
        {
            return Model.FindEntityType(typeof(TEntity)) != null
                ? base.Set<TEntity>()
                : new DocumentDbSet<TEntity>(this);
        }
    }
}
