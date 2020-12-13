using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using OrchardCore.Environment.Cache;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Modules;
using OrchardCore.Settings;
using SeedCore.Data;

namespace SeedCore.Settings.Services
{
    public class SiteService : ISiteService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISignal _signal;
        private readonly IClock _clock;
        private const string SiteCacheKey = "SiteService";

        public SiteService(
            ISignal signal,
            IMemoryCache memoryCache,
            IClock clock)
        {
            _signal = signal;
            _clock = clock;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        public IChangeToken ChangeToken => _signal.GetToken(SiteCacheKey);

        /// <inheritdoc/>
        public async Task<ISite> GetSiteSettingsAsync()
        {
            ISite site;

            if (!_memoryCache.TryGetValue(SiteCacheKey, out site))
            {
                var context = GetDbContext();
                var set = context.Set<SiteSettings>();
                site = await Task.FromResult(set.FirstOrDefault()); // session.Query<SiteSettings>().FirstOrDefaultAsync();

                if (site == null)
                {
                    lock (_memoryCache)
                    {
                        if (!_memoryCache.TryGetValue(SiteCacheKey, out site))
                        {
                            site = new SiteSettings
                            {
                                SiteSalt = Guid.NewGuid().ToString("N"),
                                SiteName = "My Application",
                                PageTitleFormat = "{% page_title Site.SiteName, position: \"after\", separator: \" - \" %}",
                                PageSize = 10,
                                MaxPageSize = 100,
                                MaxPagedCount = 0,
                                TimeZoneId = _clock.GetSystemTimeZone().TimeZoneId,
                            };
                            set.Add(site as SiteSettings);

                            _memoryCache.Set(SiteCacheKey, site);
                            _signal.SignalToken(SiteCacheKey);
                        }
                    }
                }
                else
                {
                    _memoryCache.Set(SiteCacheKey, site);
                    _signal.SignalToken(SiteCacheKey);
                }
            }

            return site;
        }


        public Task<ISite> LoadSiteSettingsAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task UpdateSiteSettingsAsync(ISite site)
        {
            var context = GetDbContext();
            var set = context.Set<SiteSettings>();

            var existing = await Task.FromResult(set.FirstOrDefault());

            existing.BaseUrl = site.BaseUrl;
            existing.PageTitleFormat = site.PageTitleFormat;
            existing.Calendar = site.Calendar;
            existing.HomeRoute = site.HomeRoute;
            existing.MaxPagedCount = site.MaxPagedCount;
            existing.MaxPageSize = site.MaxPageSize;
            existing.PageSize = site.PageSize;
            existing.Properties = site.Properties;
            existing.ResourceDebugMode = site.ResourceDebugMode;
            existing.SiteName = site.SiteName;
            existing.SiteSalt = site.SiteSalt;
            existing.SuperUser = site.SuperUser;
            existing.TimeZoneId = site.TimeZoneId;
            existing.UseCdn = site.UseCdn;
            existing.CdnBaseUrl = site.CdnBaseUrl;
            existing.AppendVersion = site.AppendVersion;

            set.Update(existing);
            context.SaveChanges();

            _memoryCache.Set(SiteCacheKey, site);
            _signal.SignalToken(SiteCacheKey);

            return;
        }

        private IDbContext GetDbContext()
        {
            return ShellScope.Services.GetService<IDbContext>();
        }

    }
}
