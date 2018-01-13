using DashboardCode.Routines.Storage;
using System.Linq;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp
{
    public class AdminkaCSharpHelper : Microsoft.EntityFrameworkCore.Design.Internal.CSharpHelper
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
