using System.Threading.Tasks;

namespace SeedCore.Data.Migrations
{
    public interface IDataMigrator
    {
        Task RunAsync(IDbContext context);
    }
}