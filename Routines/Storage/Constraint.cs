using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashboardCode.Routines.Storage
{
    public class Constraint
    {
        public const string AnnotationName = "Constraints";
        public string Name { get; set; }
        public string Message { get; set; }
        public string[] Fields { get; set; }
        public string Body { get; set; }
    }

    public static class ConstraintManager {
        public static string ProcessConstraintLiteral(Constraint[] constraints)
        {
            var type = typeof(Constraint);
            var outuput = $"new  {type.FullName}[]{{";
            foreach (var c in constraints)
            {
                var fields = string.Join(",", c.Fields.Select(e => "\"" + e + "\""));
                outuput += $"new {type.FullName}(){{Name=\"{c.Name}\", Body=@\"{c.Body}\", Message=@\"{c.Message}\", Fields=new[] {{{fields}}}}},";
            }
            outuput += "}";
            return outuput;
        }
    }
}
