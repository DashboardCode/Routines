using System.Collections.Generic;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.Storage.EfModelTest
{
    public class StorageMetaService
    {
        public List<IOrmEntitySchemaAdapter> GetStorageModels()
        {
            var list = new List<IOrmEntitySchemaAdapter> {
            };
            return list;
        }
    }
}
