using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using OrchardCore.Settings;

namespace SeedModules.Account.Routing
{
    public class LoginRouteTransformer : DynamicRouteValueTransformer
    {
        private readonly ISiteService _siteService;

        public LoginRouteTransformer(ISiteService siteService)
        {
            _siteService = siteService;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var loginRoute = (await _siteService.GetSiteSettingsAsync()).Properties["LoginRoute"];
            return new RouteValueDictionary(loginRoute?.ToObject<RouteValueDictionary>());
        }
    }
}