using System;
using System.Linq;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.AdminkaV1.Injected.Configuration;
using DashboardCode.AdminkaV1.DataAccessEfCore.Services;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.SqlServer.InstallerApp.Migrations
{
    public static class InitialCustoms
    {
        public static void Up(MigrationBuilder migrationBuilder, IModel targetModel)
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

                    var entityTypes = targetModel.GetEntityTypes();
                    foreach(var entityType in entityTypes)
                    {
                        var relationalEntityTypeAnnotations = entityType.Relational();
                        var schema = relationalEntityTypeAnnotations.Schema;
                        var tableName = relationalEntityTypeAnnotations.TableName;

                        if (entityType.FindProperty("RowVersion") != null && entityType.FindProperty("RowVersionAt") != null && entityType.FindProperty("RowVersionBy") != null)
                        {
                            migrationBuilder.Sql(
                                $"ALTER TABLE {schema}.{tableName} ADD CONSTRAINT DF_{schema}_{tableName}_RowVersionAt DEFAULT GETDATE() FOR RowVersionAt;");
                            migrationBuilder.Sql(
                                $"ALTER TABLE {schema}.{tableName} ADD CONSTRAINT DF_{schema}_{tableName}_RowVersionBy DEFAULT SUSER_SNAME() FOR RowVersionBy;");
                            migrationBuilder.Sql($"ALTER TABLE {schema}.{tableName} ADD CONSTRAINT CK_{schema}_{tableName}_RowVersionBy CHECK(RowVersionBy NOT LIKE '%[^a-z!.!-!_!\\!@]%' ESCAPE '!');");
                        }

                        var annotation = entityType.FindAnnotation("Constraints");
                        if (annotation!=null)
                        {
                            var constraints = (Constraint[])annotation.Value;
                            foreach (var c in constraints)
                            {
                                migrationBuilder.Sql($"ALTER TABLE {schema}.{tableName} ADD CONSTRAINT {c.Name} {c.Body};");
                            }
                        }
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
