using System;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;

namespace DashboardCode.Routines.ActiveDirectory
{
    public static class ActiveDirectoryManager
    {
        public static IReadOnlyList<string> ListGroups(this IIdentity identity, out string identityName, out string givenName, out string surname)
        {
            var groups = new List<string>();
            identityName = identity.Name;
            givenName = null;
            surname = null;
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                var userPrincipal = UserPrincipal.FindByIdentity(principalContext, identityName);
                givenName = userPrincipal.GivenName;
                surname = userPrincipal.Surname;
                var windowsIdentity = identity as WindowsIdentity;
                groups = windowsIdentity.Groups.Select(e => e.Translate(typeof(NTAccount)).Value).ToList();
            }
            return groups;
        }

        public static void Append(StringBuilder stringBuilder, Exception exception)
        {
            if (exception is LdapException ldapException)
                AppendLdapException(stringBuilder, ldapException);
        }

        private static void AppendLdapException(this StringBuilder stringBuilder, LdapException exception)
        {
            stringBuilder.AppendMarkdownLine("LdapException specific:");
            stringBuilder.Append("   ").AppendMarkdownProperty("ServerErrorMessage", exception.ServerErrorMessage);
            stringBuilder.Append("   ").AppendMarkdownProperty("ErrorCode", exception.ErrorCode.ToString());
            foreach (var partialResult in exception.PartialResults)
                if (partialResult != null)
                    stringBuilder.Append("   ").AppendMarkdownProperty("PartialResult", partialResult.ToString());
        }
    }
}
