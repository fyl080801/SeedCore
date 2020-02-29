using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Cache;
using OrchardCore.Modules;
using OrchardCore.Security;
using SeedCore.Data;
using SeedModules.Account.Domain;

namespace SeedModules.Roles.Services
{
    public class RoleStore : IRoleClaimStore<IRole>, IQueryableRoleStore<IRole>
    {
        private const string Key = "RolesManager.Roles";

        private readonly IDbContext _dbcontext;
        private readonly ISignal _signal;
        private readonly IMemoryCache _memoryCache;
        private readonly IServiceProvider _serviceProvider;

        public RoleStore(
            IDbContext dbcontext,
            IMemoryCache memoryCache,
            ISignal signal,
            IStringLocalizer<RoleStore> stringLocalizer,
            IServiceProvider serviceProvider,
            ILogger<RoleStore> logger)
        {
            _memoryCache = memoryCache;
            _signal = signal;
            T = stringLocalizer;
            _dbcontext = dbcontext;
            _serviceProvider = serviceProvider;
            Logger = logger;
        }

        public ILogger Logger { get; }

        public IStringLocalizer<RoleStore> T;

        public void Dispose()
        {
        }

        public IQueryable<IRole> Roles =>
            GetRolesAsync().Result.AsQueryable();

        public async Task<IEnumerable<IRole>> GetRolesAsync()
        {
            // 同一租户环境下角色数据进行缓存
            return await _memoryCache.GetOrCreateAsync(Key, async (entry) =>
            {
                var roles = await Task.FromResult(_dbcontext.Set<RoleInfo>().Select(e => (IRole)e).ToArray().AsEnumerable());

                entry.ExpirationTokens.Add(_signal.GetToken(Key));

                return roles;
            });
        }

        public void UpdateRoles(IEnumerable<IRole> roles)
        {
            _dbcontext.Set<RoleInfo>().UpdateRange(roles.Select(e => (RoleInfo)e).ToArray());
            _memoryCache.Set(Key, roles);
        }

        #region IRoleStore<IRole>
        public async Task<IdentityResult> CreateAsync(IRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            _dbcontext.Set<RoleInfo>().Add((RoleInfo)role);
            _dbcontext.SaveChanges();
            ReleaseRoles();

            return await Task.FromResult(IdentityResult.Success);
        }

        public async Task<IdentityResult> DeleteAsync(IRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            var roleinfo = (RoleInfo)role;

            if (String.Equals(roleinfo.NormalizedRolename, "ANONYMOUS") ||
                String.Equals(roleinfo.NormalizedRolename, "AUTHENTICATED"))
            {
                return IdentityResult.Failed(new IdentityError { Description = T["Can't delete system roles."] });
            }

            var roleRemovedEventHandlers = _serviceProvider.GetRequiredService<IEnumerable<IRoleRemovedEventHandler>>();
            await roleRemovedEventHandlers.InvokeAsync(x => x.RoleRemovedAsync(roleinfo.RoleName), Logger);

            var set = _dbcontext.Set<RoleInfo>();
            var oldrole = set.Find(((RoleInfo)role).Id);
            set.Remove(oldrole);
            _dbcontext.SaveChanges();
            ReleaseRoles();

            return IdentityResult.Success;
        }

        public async Task<IRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_dbcontext.Set<RoleInfo>().Find(roleId));
        }

        public async Task<IRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var role = _dbcontext.Set<RoleInfo>().FirstOrDefault(e => e.NormalizedRolename == normalizedRoleName);
            return await Task.FromResult(role);
        }

        public Task<string> GetNormalizedRoleNameAsync(IRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(((RoleInfo)role).NormalizedRolename);
        }

        public Task<string> GetRoleIdAsync(IRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.RoleName.ToUpperInvariant());
        }

        public Task<string> GetRoleNameAsync(IRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult(role.RoleName);
        }

        public Task SetNormalizedRoleNameAsync(IRole role, string normalizedName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            ((RoleInfo)role).NormalizedRolename = normalizedName;

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IRole role, string roleName, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            ((RoleInfo)role).RoleName = roleName;

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(IRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            _dbcontext.Context.Update((RoleInfo)role);
            _dbcontext.SaveChanges();
            ReleaseRoles();

            return await Task.FromResult(IdentityResult.Success);
        }

        #endregion

        #region IRoleClaimStore<IRole>
        public Task AddClaimAsync(IRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            ((RoleInfo)role).RoleClaims.Add(new RoleInfoClaim { ClaimType = claim.Type, ClaimValue = claim.Value });

            _dbcontext.Set<RoleInfo>().Update((RoleInfo)role);
            _dbcontext.SaveChanges();
            ReleaseRoles();

            return Task.CompletedTask;
        }

        public Task<IList<Claim>> GetClaimsAsync(IRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return Task.FromResult<IList<Claim>>(((RoleInfo)role).RoleClaims.Select(x => x.ToClaim()).ToList());
        }

        public Task RemoveClaimAsync(IRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            ((RoleInfo)role).RoleClaims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

            _dbcontext.Set<RoleInfo>().Update((RoleInfo)role);
            _dbcontext.SaveChanges();
            ReleaseRoles();

            return Task.CompletedTask;
        }

        #endregion

        private void ReleaseRoles()
        {
            _memoryCache.Remove(Key);
        }
    }
}
