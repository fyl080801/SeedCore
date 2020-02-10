using System;
using Microsoft.EntityFrameworkCore;

namespace SeedCore.Data
{
    public interface IDbContext : IDisposable
    {
        DbSet<MigrationRecord> Migrations { get; set; }

        DbContext Context { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        int SaveChanges();

        int SaveChanges(bool acceptAllChangesOnSuccess);
    }
}
