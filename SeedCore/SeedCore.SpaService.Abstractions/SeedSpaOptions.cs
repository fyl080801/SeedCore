using System;

namespace SeedCore.SpaService
{
    public class SeedSpaOptions
    {
        public SeedSpaOptions() : this(null)
        { }

        public SeedSpaOptions(SeedSpaOptions defaultOptions)
        {
            this.StartupTimeout = defaultOptions?.StartupTimeout ?? TimeSpan.FromSeconds(5);
            this.DefaultPage = defaultOptions?.DefaultPage ?? "/index.html";
            this.DevServerPort = defaultOptions?.DevServerPort ?? default(int);
            this.PkgManagerCommand = defaultOptions?.PkgManagerCommand ?? "npm";
        }

        public string DefaultPage { get; set; }

        public TimeSpan StartupTimeout { get; set; }

        public string PkgManagerCommand { get; set; }

        public int DevServerPort { get; set; }
    }
}