using System;
using System.Threading.Tasks;
using OrchardCore.Setup.Events;

namespace OrchardCore.Settings.Services
{
    public class SetupEventHandler : ISetupEventHandler
    {
        private readonly ISiteService _setupService;

        public SetupEventHandler(ISiteService setupService)
        {
            _setupService = setupService;
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
            Action<string, string> reportError)
        {
            var siteSettings = await _setupService.GetSiteSettingsAsync();
            siteSettings.SiteName = siteName;
            siteSettings.SuperUser = userName;
            siteSettings.TimeZoneId = siteTimeZone;
            await _setupService.UpdateSiteSettingsAsync(siteSettings);

            // TODO: Add Encryption Settings in
        }
    }
}
