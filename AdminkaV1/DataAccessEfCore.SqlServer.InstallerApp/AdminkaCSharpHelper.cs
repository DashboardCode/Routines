using System.Linq;
using Microsoft.EntityFrameworkCore.Design.Internal;

using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class AdminkaCSharpHelper : CSharpHelper
    {
        public override string UnknownLiteral(object value)
        {
            if (value is Constraint[] constraints)
            {
                var outuput = "new Constraint[]{";
                foreach (var c in constraints)
                {
                    var fields = string.Join(',', c.Fields.Select(e => "\"" + e + "\""));
                    outuput += $"new DashboardCode.Routines.Storage.Constraint(){{Name=\"{c.Name}\", Body=@\"{c.Body}\", Message=@\"{c.Message}\", Fields=new[] {{{fields}}}}},";
                }
                outuput += "}";
                return outuput;
            }
            return base.UnknownLiteral(value);
        }
    }
}
