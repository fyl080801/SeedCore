using System;

namespace SeedCore.SpaService
{
    public class SeedSpaOptions
    {
        public SeedSpaOptions() : this(null)
        { }

        public SeedSpaOptions(SeedSpaOptions defaultOptions)
        {
            this.DevServerPort = defaultOptions?.DevServerPort ?? default(int);
            this.PkgManagerCommand = defaultOptions?.PkgManagerCommand ?? "npm";
            this.SuccessRegx = defaultOptions?.SuccessRegx ?? "App running at:";
        }

        // public string DefaultPage { get; set; }

        public string PkgManagerCommand { get; set; }

        public int DevServerPort { get; set; }

        public string SuccessRegx { get; set; }
    }
}