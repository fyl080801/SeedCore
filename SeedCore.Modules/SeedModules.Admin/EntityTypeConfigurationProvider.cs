using System.Collections.Generic;
using System.Threading.Tasks;
using SeedCore.Data;
using SeedModules.Admin.Domain;

namespace SeedModules.Admin
{
    public class EntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        public async Task<IEnumerable<object>> GetEntityTypeConfigurationsAsync()
        {
            return await Task.FromResult(new object[] {
                new TestTableConfiguration()
            });
        }
    }
}