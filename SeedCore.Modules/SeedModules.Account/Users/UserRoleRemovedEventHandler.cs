using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OrchardCore.Security;
using OrchardCore.Users;

namespace SeedModules.Users
{
    public class UserRoleRemovedEventHandler : IRoleRemovedEventHandler
    {
        private readonly UserManager<IUser> _userManager;

        public UserRoleRemovedEventHandler(UserManager<IUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task RoleRemovedAsync(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);

            foreach (var user in users)
            {
                await _userManager.RemoveFromRoleAsync(user, roleName);
            }
        }
    }
}