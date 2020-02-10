using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeedCore.Data.Migrations
{
    public interface IDataMigrationManager
    {
        Task<IEnumerable<string>> GetFeaturesByUpdateAsync();

        Task UpdateAllFeaturesAsync();

        Task UpdateAsync(string featureId);

        Task UpdateAsync(IEnumerable<string> features);

        Task Uninstall(string featureId);
    }
}
