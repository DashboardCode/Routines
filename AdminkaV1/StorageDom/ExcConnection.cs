using System;

namespace AdminkaV1.StorageDom

{
    public class ExcConnection
    {
        public required string ExcConnectionId { get; set; }
        public string? ExcConnectionCode { get; set; } // short name
        public string? ExcConnectionName { get; set; }
        public string? ExcConnectionDescription { get; set; }
        public string? ExcConnectionXMeta { get; set; } // some text,
        public string? ExcConnectionType { get; set; } // sql server or parquet
        public string? ExcConnectionString { get; set; }
        public bool ExcConnectionIsActive { get; set; }
    }
}
