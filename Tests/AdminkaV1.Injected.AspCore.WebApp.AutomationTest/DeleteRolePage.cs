using System;
using System.Collections.Generic;
using System.Text;
using Atata;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.AutomationTest
{
    [Url("Auth/RoleDelete")]
    [VerifyTitle]
    [VerifyH1]
    public class DeleteRolePage : Page<DeleteRolePage>
    {
        public TextInput<DeleteRolePage> RoleId { get; private set; }
        public Button<DeleteRolePage> Delete { get; private set; }
    }
}
