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
        public static bool TryExtractUserName(this string userNameWithDomainPrefix, out string userName)
        {
            userName = null;
            if (!string.IsNullOrEmpty(userNameWithDomainPrefix))
            {
                int idx = userNameWithDomainPrefix.IndexOf('\\');
                if (idx == -1)
                    idx = userNameWithDomainPrefix.IndexOf('@');
                if (idx > -1)
                {
                    userName = userNameWithDomainPrefix.Substring(idx + 1);
                    return true;
                }
            }
            return false;
        }

        public static (string givenName, string surname) GetUserData(this IIdentity identity)
        {
            string givenName = null;
            string surname = null;
            using (var principalContext = new PrincipalContext(ContextType.Domain))
            {
                string identityName = identity.Name;
                var userPrincipal = UserPrincipal.FindByIdentity(principalContext, identityName);
                givenName = userPrincipal.GivenName;
                surname = userPrincipal.Surname;
            }
            return (givenName, surname);
        }

        public static IReadOnlyList<string> ListGroups(this WindowsIdentity windowsIdentity)
        {
            var groups = windowsIdentity.Groups.Select(e => e.Translate(typeof(NTAccount)).Value).ToList();
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
