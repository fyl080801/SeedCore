﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OrchardCore.Security;
using OrchardCore.Security.Permissions;
using SeedModules.Account.Domain;

namespace SeedModules.Account.Roles
{
    public class RolesPermissionsHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly RoleManager<IRole> _roleManager;

        public RolesPermissionsHandler(RoleManager<IRole> roleManager)
        {
            _roleManager = roleManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.HasSucceeded)
            {
                return;
            }

            var grantingNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            PermissionNames(requirement.Permission, grantingNames);

            var rolesToExamine = new List<string> { "Anonymous" };

            if (context.User.Identity.IsAuthenticated)
            {
                rolesToExamine.Add("Authenticated");
                foreach (var claim in context.User.Claims)
                {
                    if (claim.Type == ClaimTypes.Role)
                    {
                        rolesToExamine.Add(claim.Value);
                    }
                }
            }

            foreach (var roleName in rolesToExamine)
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    foreach (var claim in ((RoleInfo)role).RoleClaims)
                    {
                        if (!String.Equals(claim.ClaimType, Permission.ClaimType, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        string permissionName = claim.ClaimValue;

                        if (grantingNames.Contains(permissionName))
                        {
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }
            }
        }

        private static void PermissionNames(Permission permission, HashSet<string> stack)
        {
            stack.Add(permission.Name);

            if (permission.ImpliedBy != null && permission.ImpliedBy.Any())
            {
                foreach (var impliedBy in permission.ImpliedBy)
                {
                    if (stack.Contains(impliedBy.Name))
                    {
                        continue;
                    }

                    PermissionNames(impliedBy, stack);
                }
            }

            stack.Add(StandardPermissions.SiteOwner.Name);
        }
    }
}
