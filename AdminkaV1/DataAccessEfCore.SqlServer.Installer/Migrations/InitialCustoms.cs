using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore.Migrations;
using Vse.AdminkaV1.DomAuthentication;
using Vse.AdminkaV1.Injected;
using Vse.AdminkaV1.Injected.Configuration;

namespace Vse.AdminkaV1.DataAccessEfCore.SqlServer.Installer.Migrations
{
    public static class InitialCustoms
    {
        public static void Up(MigrationBuilder migrationBuilder)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var routine = new AdminkaRoutine(typeof(InitialCustoms).Namespace, nameof(InitialCustoms), nameof(Up), userContext, new { });

            routine.Handle(
                (state) => {
                    string loginName = null;
                    string groupAdName = "FakeDomain\\Testers";
                    var adConfiguration = state.Resolve<AdConfiguration>();
                    if (adConfiguration.UseAdAuthorization)
                    {
                        loginName = WindowsIdentity.GetCurrent().Name;
                    }
                    else
                    {
                        var fakeAdConfiguration = state.Resolve<FakeAdConfiguration>();
                        loginName = fakeAdConfiguration.FakeAdUser;
                    }
                    if (loginName == null)
                        throw new ApplicationException("login name can't be null");
                    migrationBuilder.Sql(
                        $"INSERT INTO dbo.Users (LoginName) VALUES ('{loginName}');" + Environment.NewLine
                        + $"INSERT INTO dbo.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.ConfigureSystem}','Configure System');" + Environment.NewLine
                        + $"INSERT INTO dbo.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.VerboseLogging}','VerboseLogging');" + Environment.NewLine
                        + $"INSERT INTO dbo.UsersPrivileges (UserId, PrivilegeId) VALUES (IDENT_CURRENT('dbo.Users'),'{Privilege.ConfigureSystem}');" + Environment.NewLine
                        + $"INSERT INTO dbo.Groups (GroupName, GroupAdName) VALUES ('Sample','{groupAdName}');" + Environment.NewLine
                        + $"INSERT INTO dbo.GroupsPrivileges (GroupId, PrivilegeId) VALUES (IDENT_CURRENT('dbo.Groups'),'{Privilege.VerboseLogging}');" + Environment.NewLine
                        + $"INSERT INTO dbo.UsersGroups (UserId, GroupId) VALUES (IDENT_CURRENT('dbo.Users'),IDENT_CURRENT('dbo.Groups'));" + Environment.NewLine
                    );

                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TestTypeRecords ADD CONSTRAINT CK_TestTypeRecords_TestTypeRecordName CHECK(TestTypeRecordName NOT LIKE '%[^a-z0-9 ]%');");

                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TestTypeRecords ADD CONSTRAINT DF_TestTypeRecords_RowVersionAt DEFAULT GETDATE() FOR RowVersionAt;");
                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TestTypeRecords ADD CONSTRAINT DF_TestTypeRecords_RowVersionBy DEFAULT SUSER_SNAME() FOR RowVersionBy;");


                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TestChildRecords ADD CONSTRAINT DF_TestChildRecords_RowVersionAt DEFAULT GETDATE() FOR RowVersionAt;");
                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TestChildRecords ADD CONSTRAINT DF_TestChildRecords_RowVersionBy DEFAULT SUSER_SNAME() FOR RowVersionBy;");

                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TestParentRecords ADD CONSTRAINT DF_TestParentRecords_RowVersionAt DEFAULT GETDATE() FOR RowVersionAt;");
                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TestParentRecords ADD CONSTRAINT DF_TestParentRecords_RowVersionBy DEFAULT SUSER_SNAME() FOR RowVersionBy;");
                });
        }
    }
}
