using System;
using System.Collections.Generic;
using System.Text;
using Atata;

namespace AdminkaV1.Injected.AspCore.MvcApp.Automation.NETCore.Test
{
   [Url("Auth/RoleCreate")]
   [VerifyTitle("Auth / Create Role")]
   //[VerifyH1]
   public class CreateRolePage : Page<CreateRolePage>
   {
        // default search is by label! https://atata.io/getting-started/
        public TextInput<CreateRolePage> Name { get; private set; }

        public Select<string, CreateRolePage> PrivilegesAllowed { get; private set;}
        public Select<int, CreateRolePage> PrivilegesDenied { get; private set; }

        public Button<CreateRolePage> Create { get; private set; }
   }
}
