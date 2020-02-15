using Microsoft.AspNetCore.Builder;

namespace SeedCore.SpaService
{
    public interface ISeedSpaBuilder
    {
        IApplicationBuilder ApplicationBuilder { get; }
        SeedSpaOptions Options { get; }
    }
}