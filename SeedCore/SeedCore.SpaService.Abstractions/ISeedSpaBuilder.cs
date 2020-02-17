using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices;

namespace SeedCore.SpaService
{
    public interface ISeedSpaBuilder : ISpaBuilder
    {
        SeedSpaOptions Server { get; }
        Assembly Assembly { get; }
    }
}