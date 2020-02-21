using System;

namespace SeedCore.Data.Migrations
{
    public class MigrationRecord
    {
        public int Id { get; set; }

        public string SnapshotDefine { get; set; }

        public DateTime MigrationTime { get; set; }
    }
}
