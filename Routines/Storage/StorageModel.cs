namespace DashboardCode.Routines.Storage
{
    public class StorageModel
    {
        public const string GenericErrorField = "";
        public Entity Entity { get; set; }
        public string TableName { get; set; }
        public string[] Requireds { get; set; } 
        public Key Key { get; set; }
        public string[] Binaries {get;set;}
        public Unique[] Uniques { get; set; }
        public Constraint[] Constraints { get; set; }

    }

    public class Entity
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
    }
    public class Key
    {
        public string[] Attributes { get; set; }
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
    }

    public class Required
    {
        public string Attribute { get; set; }
    }
}
