using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Migrations;

using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.AdminkaV1.Injected.Configuration;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp.Migrations
{
    public static class InitialCustoms
    {
        public static void Up(MigrationBuilder migrationBuilder)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var installerApplicationFactory = new InstallerApplicationFactory();
            var routine = new AdminkaRoutineHandler(typeof(InitialCustoms).Namespace, nameof(InitialCustoms), nameof(Up), userContext, installerApplicationFactory, new { });

            routine.Handle(
                (state) => {
                    string loginName = null;
                    string groupAdName = "FakeDomain\\Testers";
                    var adConfiguration = state.Resolve<AdConfiguration>();
                    if (adConfiguration.UseAdAuthorization)
                    {
                        // TODO: Core 2.1 will contains AD functionality https://github.com/dotnet/corefx/issues/2089 and 
                        // there I will need to update this code to get name from AD similar to WindowsIdentity.GetCurrent().Name;
                        loginName = Environment.UserDomainName + "\\" + Environment.UserName;
                    }
                    else
                    {
                        var fakeAdConfiguration = state.Resolve<FakeAdConfiguration>();
                        loginName = fakeAdConfiguration.FakeAdUser;
                    }
                    if (loginName == null)
                        throw new Exception("login name can't be null");

                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TypeRecords ADD CONSTRAINT CK_tst_TypeRecords_TypeRecordName CHECK(TypeRecordName NOT LIKE '%[^a-z0-9 ]%');");

                    migrationBuilder.Sql("ALTER TABLE scr.Privileges ADD CONSTRAINT CK_scr_Privileges_PrivilegeId CHECK(PrivilegeId NOT LIKE '%[^a-z0-9]%');");

                    migrationBuilder.Sql(@"ALTER TABLE scr.Users ADD CONSTRAINT CK_scr_Users_FirstName CHECK(FirstName NOT LIKE '%[^a-z ]%');");
                    migrationBuilder.Sql(@"ALTER TABLE scr.Users ADD CONSTRAINT CK_scr_Users_SecondName CHECK(SecondName NOT LIKE '%[^a-z '']%');");
                    migrationBuilder.Sql(@"ALTER TABLE scr.Users ADD CONSTRAINT CK_scr_Users_LoginName CHECK(LoginName NOT LIKE '%[^a-z!.!-!_!\!@]%' ESCAPE '!');");

                    migrationBuilder.Sql(@"ALTER TABLE scr.Roles ADD CONSTRAINT CK_scr_Roles_RoleName CHECK(RoleName NOT LIKE '%[^a-z0-9 ]%');");
                    migrationBuilder.Sql(@"ALTER TABLE scr.Groups ADD CONSTRAINT CK_scr_Groups_GroupName CHECK(GroupName NOT LIKE '%[^a-z0-9 ]%');");
                    migrationBuilder.Sql(@"ALTER TABLE scr.Groups ADD CONSTRAINT CK_scr_Groups_GroupAdName CHECK(GroupAdName NOT LIKE '%[^a-z!.!-!_!\!@]%' ESCAPE '!');");

                    ValueTuple<string, string>[] tables = new[] { ("tst", "TypeRecords"), ("tst", "ChildRecords"), ("tst", "ParentRecords")
                            ,("scr","Users"),("scr","Privileges"),("scr","Groups"),("scr","Roles")
                            ,("scr","GroupPrivilegeMap"),("scr","GroupRoleMap"),("scr","RolePrivilegeMap")
                            ,("scr","UserGroupMap"),("scr","UserPrivilegeMap"),("scr","UserRoleMap")
                    };
                    foreach ( var (schema,table) in tables)
                    {
                        migrationBuilder.Sql(
                            $"ALTER TABLE {schema}.{table} ADD CONSTRAINT DF_{schema}_{table}_RowVersionAt DEFAULT GETDATE() FOR RowVersionAt;");
                        migrationBuilder.Sql(
                            $"ALTER TABLE {schema}.{table} ADD CONSTRAINT DF_{schema}_{table}_RowVersionBy DEFAULT SUSER_SNAME() FOR RowVersionBy;");
                        migrationBuilder.Sql($"ALTER TABLE {schema}.{table} ADD CONSTRAINT CK_{schema}_{table}_RowVersionBy CHECK(RowVersionBy NOT LIKE '%[^a-z!.!-!_!\\!@]%' ESCAPE '!');");
                    }

                    migrationBuilder.Sql(
                          $"INSERT INTO scr.Users (LoginName) VALUES ('{loginName}');" + Environment.NewLine
                        + $"INSERT INTO scr.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.ConfigureSystem}','Configure System');" + Environment.NewLine
                        + $"INSERT INTO scr.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.VerboseLogging}','Verbose Logging');" + Environment.NewLine
                        + $"INSERT INTO scr.UserPrivilegeMap (UserId, PrivilegeId) VALUES (IDENT_CURRENT('scr.Users'),'{Privilege.ConfigureSystem}');" + Environment.NewLine
                        + $"INSERT INTO scr.Groups (GroupName, GroupAdName) VALUES ('Sample','{groupAdName}');" + Environment.NewLine
                        + $"INSERT INTO scr.GroupPrivilegeMap (GroupId, PrivilegeId) VALUES (IDENT_CURRENT('scr.Groups'),'{Privilege.VerboseLogging}');" + Environment.NewLine
                        + $"INSERT INTO scr.UserGroupMap (UserId, GroupId) VALUES (IDENT_CURRENT('scr.Users'),IDENT_CURRENT('scr.Groups'));" + Environment.NewLine
                        + $"INSERT INTO scr.Roles (RoleName) VALUES ('Administrator');" + Environment.NewLine
                        + $"INSERT INTO scr.RolePrivilegeMap (RoleId, PrivilegeId) VALUES (IDENT_CURRENT('scr.Roles'),'{Privilege.ConfigureSystem}');" + Environment.NewLine
                    );
                });
        }
    }
}
