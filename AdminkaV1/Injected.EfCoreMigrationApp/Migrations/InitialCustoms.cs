using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

using DashboardCode.Routines;

using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.ActiveDirectoryServices;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp
{
    public static class InitialCustoms
    {
        public static void Up(MigrationBuilder migrationBuilder, IModel targetModel)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = builder.Build();
            var adminkaDbInstallGroups = new List<AdminkaDbInstallGroup>();
            config.GetSection("AdminkaDbInstallGroups").Bind(adminkaDbInstallGroups);

            var userContext = new UserContext("EFCoreMigrations", CultureInfo.CurrentCulture);

            var routine = new AdminkaRoutineHandler(
                Program.ApplicationSettings.AdminkaStorageConfiguration,
                Program.ApplicationSettings.PerformanceCounters,
                Program.ApplicationSettings.AuthenticationLogging,
                Program.ApplicationSettings.ConfigurationContainerFactory,
                new MemberTag(typeof(InitialCustoms).Namespace, nameof(InitialCustoms), nameof(Up)), userContext,
                new { });

            routine.UserRoutineHandler.Handle(
                closure => {
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
                    // 'FakeDomain\\\\Administrators', 'FakeDomain\\\\Testers'
                    // Create privileges
                    var sql = 
                        $"INSERT INTO scr.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.ConfigureSystem}','Configure System');" + Environment.NewLine
                        + $"INSERT INTO scr.Privileges (PrivilegeId, PrivilegeName) VALUES ('{Privilege.VerboseLogging}','Verbose Logging');" + Environment.NewLine;
                    // Create roles for privileges
                    sql += $"INSERT INTO scr.Roles (RoleName) VALUES ('Administrator');" + Environment.NewLine
                        + $"INSERT INTO scr.RolePrivilegeMap (RoleId, PrivilegeId) VALUES (IDENT_CURRENT('scr.Roles'),'{Privilege.ConfigureSystem}');" + Environment.NewLine
                        + $"INSERT INTO scr.Roles (RoleName) VALUES ('Tester');" + Environment.NewLine
                        + $"INSERT INTO scr.RolePrivilegeMap (RoleId, PrivilegeId) VALUES (IDENT_CURRENT('scr.Roles'),'{Privilege.VerboseLogging}');" + Environment.NewLine;

                    //  create current user, give him 'Configuration' privilege
                    sql += $"INSERT INTO scr.Users (LoginName) VALUES ('{loginName}');" + Environment.NewLine
                        + $"INSERT INTO scr.UserPrivilegeMap (UserId, PrivilegeId) VALUES (IDENT_CURRENT('scr.Users'),'{Privilege.ConfigureSystem}');" + Environment.NewLine;

                    // create groups (add priveleges)
                    foreach (var g in adminkaDbInstallGroups)
                    {
                        sql += $"INSERT INTO scr.Groups (GroupName, GroupAdName) VALUES ('{g.Name}','{g.Name}');" + Environment.NewLine;
                        foreach (var privilegeId in g.Priveleges)
                            sql += $"INSERT INTO scr.GroupPrivilegeMap (GroupId, PrivilegeId) VALUES (IDENT_CURRENT('scr.Groups'),'{privilegeId}');" + Environment.NewLine;
                    }
                    migrationBuilder.Sql(sql);
                });
        }
    }
}