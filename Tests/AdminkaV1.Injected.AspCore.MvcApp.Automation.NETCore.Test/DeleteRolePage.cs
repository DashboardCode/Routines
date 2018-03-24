using System;
using System.Collections.Generic;
using System.Text;
using Atata;

namespace AdminkaV1.Injected.AspCore.MvcApp.Automation.NETCore.Test
{
    [Url("Roles/Delete")]
    [VerifyTitle]
    [VerifyH1]
    public class DeleteRolePage : Page<DeleteRolePage>
    {
        public TextInput<DeleteRolePage> RoleId { get; private set; }
        public Button<DeleteRolePage> Delete { get; private set; }
    }
}
