using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminkaV1.StorageDom
{
    public class ExcTable
    {
        public required string ExcTableId { get; set; }
        public string? ExcTableDescription { get; set; }
        public string? ExcTableXMeta { get; set; } // will hold data about tables and fields
        public string? ExcTablePath { get; set; } // Path (in case of sql server will be full table name, in parquet – file location).
        public string? ExcTableFields { get; set; } //  (JSON/XML list of fields). Name/Type/Description/XMeta. (This one can be a separate table, but that may be too much).
        public required string? ExcConnectionId { get; set; } // reference to ExcConnection
    }
}
