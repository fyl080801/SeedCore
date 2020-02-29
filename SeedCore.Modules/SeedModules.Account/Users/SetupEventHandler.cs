using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OrchardCore.Security;
using OrchardCore.Security.Services;
using OrchardCore.Setup.Events;
using OrchardCore.Users.Services;
using SeedCore.Data;
using SeedModules.Account.Domain;

namespace SeedModules.Users
{
    public class SetupEventHandler : ISetupEventHandler
    {
        private readonly IUserService _userService;
        private readonly IDbContext _dbcontext;

        public SetupEventHandler(IUserService userService, IDbContext dbcontext)
        {
            _userService = userService;
            _dbcontext = dbcontext;
        }

        public async Task Setup(
            string siteName,
            string userName,
            string email,
            string password,
            string dbProvider,
            string dbConnectionString,
            string dbTablePrefix,
            string siteTimeZone,
            Action<string, string> reportError
            )
        {
            var role = _dbcontext.Set<RoleInfo>().FirstOrDefault(e => e.RoleName == "Administrator");

            var user = new User
            {
                UserName = userName,
                Email = email,
                EmailConfirmed = true
            };

            if (role != null)
            {
                user.Roles.Add(new UserRole()
                {
                    RoleId = role.Id
                });
            }

            await _userService.CreateUserAsync(user, password, reportError);
        }
    }
}