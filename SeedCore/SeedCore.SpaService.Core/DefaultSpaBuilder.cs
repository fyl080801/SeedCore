using Microsoft.AspNetCore.Builder;

namespace SeedCore.SpaService
{
    public class DefaultSpaBuilder : ISeedSpaBuilder
    {
        public IApplicationBuilder ApplicationBuilder => throw new System.NotImplementedException();

        public SeedSpaOptions Options => throw new System.NotImplementedException();
    }
}