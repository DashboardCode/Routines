using System;
using System.Collections.Generic;
using System.Text;
using Atata;

namespace AdminkaV1.Injected.AspCore.MvcApp.Automation.NETCore.Test
{
   [Url("Roles/Create")]
   [VerifyTitle]
   [VerifyH1]
   public class CreateRolePage : Page<CreateRolePage>
   {
       public TextInput<CreateRolePage> RoleName { get; private set; }

       public OptionList<string, CreateRolePage> Privileges { get; private set;}
       public OptionList<int, CreateRolePage> Groups { get; private set; }

       public Button<CreateRolePage> Create { get; private set; }
   }
}
