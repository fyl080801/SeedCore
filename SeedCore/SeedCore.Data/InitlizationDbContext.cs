using Microsoft.EntityFrameworkCore;

namespace SeedCore.Data
{
    public class InitlizationDbContext : DbContext
    {
        public InitlizationDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}