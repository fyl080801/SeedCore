using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeedCore.Data
{
    public interface IEntityTypeConfigurationProvider
    {
        Task<IEnumerable<object>> GetEntityTypeConfigurationsAsync();
    }
}