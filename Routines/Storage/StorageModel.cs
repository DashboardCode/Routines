namespace DashboardCode.Routines.Storage
{
    public class StorageModel
    {
        public Entity Entity { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string[] Keys { get; set; }
        public string[] Requireds { get; set; } 
        public string[] Binaries {get;set;}
        public Unique[] Uniques { get; set; }
        public Constraint[] Constraints { get; set; }
    }

    public class Entity
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
    }

    public class Unique
    {
        public string IndexName { get; set; }
        public string[] Fields { get; set; }
    }

    public class Constraint
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string[] Fields { get; set; }
        public string Body { get; set; }
    }
}