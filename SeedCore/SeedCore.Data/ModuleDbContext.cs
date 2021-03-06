﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrchardCore.Environment.Shell;
using SeedCore.Data.Migrations;

namespace SeedCore.Data
{
    public class ModuleDbContext : DbContext, IDbContext
    {
        readonly IEnumerable<object> _entityConfigurations;
        readonly ShellSettings _settings;

        public DbContext Context => this;

        public DbSet<Document> Document { get; set; }

        public DbSet<MigrationRecord> Migrations { get; set; }

        public ModuleDbContext(
            DbContextOptions options,
            ShellSettings settings,
            IEnumerable<object> entityConfigurations)
            : base(options)
        {
            _entityConfigurations = entityConfigurations;
            _settings = settings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MigrationRecordTypeConfiguration());

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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        public override DbSet<TEntity> Set<TEntity>()
        {
            return Model.FindEntityType(typeof(TEntity)) != null
                ? base.Set<TEntity>()
                : new DocumentDbSet<TEntity>(this);
        }
    }
}
