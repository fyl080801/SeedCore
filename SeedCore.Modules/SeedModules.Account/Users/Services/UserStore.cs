using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using OrchardCore.Modules;
using OrchardCore.Security;
using OrchardCore.Security.Services;
using OrchardCore.Users;
using OrchardCore.Users.Handlers;
using SeedCore.Data;
using SeedModules.Account.Domain;

namespace SeedModules.Users.Services
{
    public class UserStore :
        IUserClaimStore<IUser>,
        IUserRoleStore<IUser>,
        IUserPasswordStore<IUser>,
        IUserEmailStore<IUser>,
        IUserSecurityStampStore<IUser>,
        IUserLoginStore<IUser>
    {
        private readonly IDbContext _dbcontext;
        private readonly IRoleService _roleService;
        private readonly IRoleClaimStore<IRole> _roleClaimStore;
        private readonly ILookupNormalizer _keyNormalizer;
        private readonly ILogger _logger;

        public UserStore(
            IDbContext dbcontext,
            IRoleService roleService,
            IRoleClaimStore<IRole> roleClaimStore,
            ILookupNormalizer keyNormalizer,
            ILogger<UserStore> logger,
            IEnumerable<IUserCreatedEventHandler> handlers)
        {
            _dbcontext = dbcontext;
            _roleService = roleService;
            _roleClaimStore = roleClaimStore;
            _keyNormalizer = keyNormalizer;
            _logger = logger;
            Handlers = handlers;
        }

        public IEnumerable<IUserCreatedEventHandler> Handlers { get; private set; }

        public void Dispose()
        {
        }

        public string NormalizeKey(string key)
        {
            return _keyNormalizer == null ? key : _keyNormalizer.NormalizeName(key);
        }

        #region IUserStore<IUser>
        public async Task<IdentityResult> CreateAsync(IUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                var newuser = (User)user;
                _dbcontext.Set<User>().Add(newuser);
                _dbcontext.SaveChanges();

                var context = new CreateUserContext(user);
                await Handlers.InvokeAsync(handler => handler.CreatedAsync(context), _logger);
            }
            catch
            {
                return IdentityResult.Failed();
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(IUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                var userSet = _dbcontext.Set<User>();
                var olduser = userSet.Find(((User)user).Id);
                userSet.Remove(olduser);
                _dbcontext.SaveChanges();
            }
            catch
            {
                return await Task.FromResult(IdentityResult.Failed());
            }

            return await Task.FromResult(IdentityResult.Success);
        }

        public async Task<IUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            int id;
            if (!int.TryParse(userId, out id))
            {
                return null;
            }

            var user = _dbcontext.Set<User>().Find(Convert.ToInt32(userId));

            return await Task.FromResult(user);
        }

        public async Task<IUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = _dbcontext.Set<User>().FirstOrDefault(e => e.UserName == normalizedUserName);
            return await Task.FromResult<IUser>(query);
        }

        public Task<string> GetNormalizedUserNameAsync(IUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(((User)user).NormalizedUsername);
        }

        public Task<string> GetUserIdAsync(IUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(((User)user).Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public Task<string> GetUserNameAsync(IUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(((User)user).UserName);
        }

        public Task SetNormalizedUserNameAsync(IUser user, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            ((User)user).NormalizedUsername = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            ((User)user).UserName = userName;

            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _dbcontext.Context.Update((User)user);
            _dbcontext.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        #endregion

        #region IUserPasswordStore<IUser>
        public Task<string> GetPasswordHashAsync(IUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(((User)user).PasswordHash);
        }

        public Task SetPasswordHashAsync(IUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            ((User)user).PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task<bool> HasPasswordAsync(IUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(((User)user).PasswordHash != null);
        }

        #endregion

        #region ISecurityStampValidator<IUser>
        public Task SetSecurityStampAsync(IUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            ((User)user).SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(IUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(((User)user).SecurityStamp);
        }
        #endregion

        #region IUserEmailStore<IUser>
        public Task SetEmailAsync(IUser user, string email, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            ((User)user).Email = email;

            return Task.CompletedTask;
        }

        public Task<string> GetEmailAsync(IUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(((User)user).Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(((User)user).EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(IUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            ((User)user).EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public async Task<IUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var query = _dbcontext.Set<User>().FirstOrDefault(e => e.NormalizedEmail == normalizedEmail);
            return await Task.FromResult<IUser>(query);
        }

        public Task<string> GetNormalizedEmailAsync(IUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(((User)user).NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(IUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            ((User)user).NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        #endregion

        #region IUserRoleStore<IUser>
        public async Task AddToRoleAsync(IUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roleNames = await _roleService.GetRoleNamesAsync();
            var roleName = roleNames?.FirstOrDefault(r => NormalizeKey(r) == normalizedRoleName);

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new InvalidOperationException($"Role {normalizedRoleName} does not exist.");
            }
            var olduser = _dbcontext.Set<User>().Find(((User)user).Id);
            var role = (RoleInfo)(await _roleClaimStore.FindByNameAsync(normalizedRoleName, cancellationToken));
            if (olduser.Roles.Count(e => e.Role.Id == role.Id) <= 0)
            {
                olduser.Roles.Add(new UserRole()
                {
                    UserId = olduser.Id,
                    RoleId = role.Id
                });
            }
            _dbcontext.SaveChanges();
        }

        public async Task RemoveFromRoleAsync(IUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var roleNames = await _roleService.GetRoleNamesAsync();
            var roleName = roleNames?.FirstOrDefault(r => NormalizeKey(r) == normalizedRoleName);

            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new InvalidOperationException($"Role {normalizedRoleName} does not exist.");
            }

            var exuser = _dbcontext.Set<User>().Find(((User)user).Id);
            var userrole = exuser.Roles.FirstOrDefault(e => e.Role.RoleName == roleName);
            exuser.Roles.Remove(userrole);
            _dbcontext.SaveChanges();
        }

        public Task<IList<string>> GetRolesAsync(IUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult<IList<string>>(((User)user).Roles.Select(e => e.Role.RoleName).ToArray());
        }

        public Task<bool> IsInRoleAsync(IUser user, string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (string.IsNullOrWhiteSpace(normalizedRoleName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(normalizedRoleName));
            }

            return Task.FromResult(((User)user).Roles.Any(e => e.Role.NormalizedRolename.Equals(normalizedRoleName)));
        }

        public async Task<IList<IUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(normalizedRoleName))
            {
                throw new ArgumentNullException(nameof(normalizedRoleName));
            }

            var role = _dbcontext.Set<RoleInfo>().FirstOrDefault(e => e.NormalizedRolename == normalizedRoleName);
            var user = _dbcontext.Set<User>();
            if (role == null)
            {
                throw new Exception($"Not exist role normalized name '{normalizedRoleName}'");
            }
            return await Task.FromResult<IList<IUser>>(user.Where(e => e.Roles.Select(r => r.RoleId).Contains(role.Id)).Select(e => (IUser)e).ToList());
        }
        #endregion

        #region IUserLoginStore<IUser>
        public Task AddLoginAsync(IUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }

            if (((User)user).UserLogins.Any(i => i.LoginProvider == login.LoginProvider))
                throw new InvalidOperationException($"Provider {login.LoginProvider} is already linked for {user.UserName}");

            ((User)user).UserLogins.Add(new LoginInfo()
            {
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            });
            _dbcontext.SaveChanges();

            return Task.CompletedTask;
        }

        public async Task<IUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_dbcontext.Set<User>().FirstOrDefault(e => e.UserLogins.Any(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey)));
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(IUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult<IList<UserLoginInfo>>(((User)user).UserLogins.Select(e => new UserLoginInfo(e.LoginProvider, e.ProviderKey, e.ProviderDisplayName)).ToArray());
        }

        public Task RemoveLoginAsync(IUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var set = _dbcontext.Set<LoginInfo>();
            var userinfo = (User)user;
            var externalLogin = set.FirstOrDefault(e => e.UserId == userinfo.Id && e.LoginProvider == loginProvider && e.ProviderKey == providerKey);
            if (externalLogin != null)
            {
                set.Remove(externalLogin);
                _dbcontext.SaveChanges();
            }

            return Task.CompletedTask;
        }

        #endregion

        #region IUserClaimStore<IUser>
        public Task<IList<Claim>> GetClaimsAsync(IUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult<IList<Claim>>(((User)user).UserClaims.Select(x => x.ToClaim()).ToList());
        }

        public Task AddClaimsAsync(IUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            foreach (var claim in claims)
            {
                ((User)user).UserClaims.Add(new UserClaim { ClaimType = claim.Type, ClaimValue = claim.Value });
            }

            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(IUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));
            if (newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

            foreach (var userClaim in ((User)user).UserClaims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type))
            {
                userClaim.ClaimValue = newClaim.Value;
                userClaim.ClaimType = newClaim.Type;
            }

            return Task.CompletedTask;
        }

        public Task RemoveClaimsAsync(IUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            foreach (var claim in claims)
            {
                foreach (var userClaim in ((User)user).UserClaims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToList())
                    ((User)user).UserClaims.Remove(userClaim);
            }

            return Task.CompletedTask;
        }

        public async Task<IList<IUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            var users = _dbcontext.Set<User>().Where(e => e.UserClaims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value));
            return await Task.FromResult(users.Cast<IUser>().ToList());
        }
        #endregion
    }
}
