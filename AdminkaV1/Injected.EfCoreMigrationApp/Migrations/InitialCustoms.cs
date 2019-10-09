using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

using DashboardCode.Routines.Storage.EfCore.Relational;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.Injected.ActiveDirectory;

namespace DashboardCode.AdminkaV1.Injected.EfCoreMigrationApp
{
    static class InitialCustoms
    {
        public static void Up(MigrationBuilder migrationBuilder, IModel targetModel)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            var adminkaDbInstallGroups = new List<AdminkaDbInstallGroup>();
            config.GetSection("AdminkaDbInstallGroups").Bind(adminkaDbInstallGroups);

            var routine = new AdminkaAnonymousRoutineHandler(
                    Program.ApplicationSettings,
                    "EFCoreMigrations",
                    new { },
                    correlationToken: System.Guid.NewGuid(),
                    documentBuilder: null,
                    controllerNamespace: null,
                    controllerName: nameof(InitialCustoms)
                );

            routine.Handle(
                (container,closure) => {
                    EfCoreRelationalManager.ProcessTargetModel(migrationBuilder, targetModel); 
                    


                    string loginName = null;
                    // string groupAdName = "FakeDomain\\Testers";
                    var adConfiguration = closure.Resolve<AdConfiguration>();
                    if (adConfiguration.UseAdAuthorization)
                    {
                        // TODO: Core 2.1 will contains AD functionality https://github.com/dotnet/corefx/issues/2089 and 
                        // there I will need to update this code to get name from AD similar to WindowsIdentity.GetCurrent().Name;
                        loginName = Environment.UserDomainName + "\\" + Environment.UserName;
                    }
                    else
                    {
                        var fakeAdConfiguration = closure.Resolve<FakeAdConfiguration>();
                        loginName = fakeAdConfiguration.FakeAdUser;
                    }
                    if (loginName == null)
                        throw new Exception("login name can't be null");
                    // 'FakeDomain\\\\Administrators', 'FakeDomain\\\\Testers'
                    // Create privileges
                    var sql = 
                        $"INSERT INTO scr.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.ConfigureSystem}','Configure System');" + Environment.NewLine
                        + $"INSERT INTO scr.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.VerboseLogging}','Verbose Logging');" + Environment.NewLine;
                    // Create roles for privileges
                    sql += $"INSERT INTO scr.Roles (RoleName) VALUES ('Administrator');" + Environment.NewLine
                        + $"INSERT INTO scr.RolePrivilegeMap (RoleId, PrivilegeId, IsAllowed) VALUES (IDENT_CURRENT('scr.Roles'),'{Privilege.ConfigureSystem}', 1);" + Environment.NewLine
                        + $"INSERT INTO scr.Roles (RoleName) VALUES ('Tester');" + Environment.NewLine
                        + $"INSERT INTO scr.RolePrivilegeMap (RoleId, PrivilegeId, IsAllowed) VALUES (IDENT_CURRENT('scr.Roles'),'{Privilege.VerboseLogging}', 1);" + Environment.NewLine;

                    //  create current user, give him 'Configuration' privilege
                    sql += $"INSERT INTO scr.Users (LoginName) VALUES ('{loginName}');" + Environment.NewLine
                        + $"INSERT INTO scr.UserPrivilegeMap (UserId, PrivilegeId, IsAllowed) VALUES (IDENT_CURRENT('scr.Users'),'{Privilege.ConfigureSystem}', 1);" + Environment.NewLine;

                    // create groups (add priveleges)
                    foreach (var g in adminkaDbInstallGroups)
                    {
                        sql += $"INSERT INTO scr.Groups (GroupName, GroupAdName) VALUES ('{g.Name}','{g.Name}');" + Environment.NewLine;
                        foreach (var privilegeId in g.Priveleges)
                            sql += $"INSERT INTO scr.GroupPrivilegeMap (GroupId, PrivilegeId, IsAllowed) VALUES (IDENT_CURRENT('scr.Groups'),'{privilegeId}', 1);" + Environment.NewLine;
                    }
                    migrationBuilder.Sql(sql);
                });
        }
    }
}