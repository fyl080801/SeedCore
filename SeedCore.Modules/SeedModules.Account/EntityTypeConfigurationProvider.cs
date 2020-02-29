using SeedCore.Data;
using SeedModules.Account.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeedModules.Account
{
    public class EntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        public Task<IEnumerable<object>> GetEntityTypeConfigurationsAsync()
        {
            return Task.FromResult(new object[]
            {
                new RoleTypeConfiguration(),
                new RoleClaimTypeConfiguration(),
                new UserRoleTypeConfiguration(),
                new UserRoleTypeConfiguration(),
                new LoginInfoTypeConfiguration()
            }.AsEnumerable());
        }
    }
}