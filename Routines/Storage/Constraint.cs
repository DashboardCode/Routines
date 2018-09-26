using System;
using System.Collections.Generic;
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
}
