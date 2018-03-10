using System;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration.NETStandard;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.ActiveDirectoryServices;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp
{
    public static class InitialCustoms
    {
        public static void Up(MigrationBuilder migrationBuilder, IModel targetModel)
        {
            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);
            var configurationLoader = new ConfigurationManagerLoader();
            var sqlServerAdmikaConfigurationFacade = new SqlServerAdmikaConfigurationFacade(configurationLoader);
            var configurationFactory = new ConfigurationContainerFactory(configurationLoader); 
            var routine = new AdminkaRoutineHandler(
                sqlServerAdmikaConfigurationFacade.ResolveAdminkaStorageConfiguration(),
                configurationFactory,
                new MemberTag(typeof(InitialCustoms).Namespace, nameof(InitialCustoms), nameof(Up)), userContext,
                new { });

            routine.Handle(
                closure => {
                    string loginName = null;
                    string groupAdName = "FakeDomain\\Testers";
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

                        var annotation = entityType.FindAnnotation(Constraint.AnnotationName);
                        if (annotation != null)
                        {
                            var constraints = (Constraint[])annotation.Value;
                            foreach (var c in constraints)
                            {
                                var s = $"ALTER TABLE {schema}.{tableName} ADD CONSTRAINT {c.Name} {c.Body};";
                                migrationBuilder.Sql(s);
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