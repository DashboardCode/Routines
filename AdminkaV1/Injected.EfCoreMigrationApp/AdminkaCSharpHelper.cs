using System.Linq;
using DashboardCode.Routines.Storage;
using Microsoft.EntityFrameworkCore.Design.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp
{
    public class AdminkaCSharpHelper : CSharpHelper
    {
        public AdminkaCSharpHelper(IRelationalTypeMappingSource relationalTypeMappingSource) : base(relationalTypeMappingSource)
        {

        }

        public override string UnknownLiteral(object value)
        {
            if (value is Constraint[] constraints)
            {
                var type = typeof(Constraint);
                var outuput = $"new  {type.FullName}[]{{";
                foreach (var c in constraints)
                {
                    var fields = string.Join(',', c.Fields.Select(e => "\"" + e + "\""));
                    outuput += $"new {type.FullName}(){{Name=\"{c.Name}\", Body=@\"{c.Body}\", Message=@\"{c.Message}\", Fields=new[] {{{fields}}}}},";
                }
                outuput += "}";
                return outuput;
            }
            return base.UnknownLiteral(value);
        }
    }
}
