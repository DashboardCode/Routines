using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore.Migrations;
using DashboardCode.AdminkaV1.DomAuthentication;
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
            var routine = new AdminkaRoutine(typeof(InitialCustoms).Namespace, nameof(InitialCustoms), nameof(Up), userContext, installerApplicationFactory, new { });

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
                        throw new Exception("login name can't be null");
                    migrationBuilder.Sql(
                        $"INSERT INTO dbo.Users (LoginName) VALUES ('{loginName}');" + Environment.NewLine
                        + $"INSERT INTO dbo.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.ConfigureSystem}','Configure System');" + Environment.NewLine
                        + $"INSERT INTO dbo.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.VerboseLogging}','VerboseLogging');" + Environment.NewLine
                        + $"INSERT INTO dbo.UserPrivilegeMap (UserId, PrivilegeId) VALUES (IDENT_CURRENT('dbo.Users'),'{Privilege.ConfigureSystem}');" + Environment.NewLine
                        + $"INSERT INTO dbo.Groups (GroupName, GroupAdName) VALUES ('Sample','{groupAdName}');" + Environment.NewLine
                        + $"INSERT INTO dbo.GroupPrivilegeMap (GroupId, PrivilegeId) VALUES (IDENT_CURRENT('dbo.Groups'),'{Privilege.VerboseLogging}');" + Environment.NewLine
                        + $"INSERT INTO dbo.UserGroupMap (UserId, GroupId) VALUES (IDENT_CURRENT('dbo.Users'),IDENT_CURRENT('dbo.Groups'));" + Environment.NewLine
                    );

                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TypeRecords ADD CONSTRAINT CK_TypeRecords_TypeRecordName CHECK(TypeRecordName NOT LIKE '%[^a-z0-9 ]%');");

                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TypeRecords ADD CONSTRAINT DF_TypeRecords_RowVersionAt DEFAULT GETDATE() FOR RowVersionAt;");
                    migrationBuilder.Sql(
                        "ALTER TABLE tst.TypeRecords ADD CONSTRAINT DF_TypeRecords_RowVersionBy DEFAULT SUSER_SNAME() FOR RowVersionBy;");


                    migrationBuilder.Sql(
                        "ALTER TABLE tst.ChildRecords ADD CONSTRAINT DF_ChildRecords_RowVersionAt DEFAULT GETDATE() FOR RowVersionAt;");
                    migrationBuilder.Sql(
                        "ALTER TABLE tst.ChildRecords ADD CONSTRAINT DF_ChildRecords_RowVersionBy DEFAULT SUSER_SNAME() FOR RowVersionBy;");

                    migrationBuilder.Sql(
                        "ALTER TABLE tst.ParentRecords ADD CONSTRAINT DF_ParentRecords_RowVersionAt DEFAULT GETDATE() FOR RowVersionAt;");
                    migrationBuilder.Sql(
                        "ALTER TABLE tst.ParentRecords ADD CONSTRAINT DF_ParentRecords_RowVersionBy DEFAULT SUSER_SNAME() FOR RowVersionBy;");
                });
        }
    }
}
