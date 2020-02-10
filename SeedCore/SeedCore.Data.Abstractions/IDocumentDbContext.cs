using Microsoft.EntityFrameworkCore;

namespace SeedCore.Data
{
    public interface IDocumentDbContext : IDbContext
    {
        DbSet<Document> Document { get; set; }
    }
}
