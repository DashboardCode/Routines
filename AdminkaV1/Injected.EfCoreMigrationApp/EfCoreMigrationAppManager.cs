//using DashboardCode.Routines.Storage;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;
//using Microsoft.EntityFrameworkCore.Migrations;

//namespace DashboardCode.AdminkaV1.Injected.EfCoreMigrationApp
//{
//    public static class EfCoreMigrationAppManager
//    {
//        public static void ProcessTargetModel(this MigrationBuilder migrationBuilder, IModel targetModel)
//        {
//            var entityTypes = targetModel.GetEntityTypes();
//            foreach (var entityType in entityTypes)
//            {
//                var relationalEntityTypeAnnotations = entityType.Relational();
//                var schema = relationalEntityTypeAnnotations.Schema;
//                var tableName = relationalEntityTypeAnnotations.TableName;

//                if (entityType.FindProperty("RowVersion") != null
//                    && entityType.FindProperty("RowVersionAt") != null
//                    && entityType.FindProperty("RowVersionBy") != null)
//                {
//                    EfCoreMigrationAppManager.ProcessRowVersion(migrationBuilder, schema, tableName);
//                }

//                var annotation = entityType.FindAnnotation(Constraint.AnnotationName);
//                if (annotation != null && annotation.Value is Constraint[] constraints)
//                {
//                    EfCoreMigrationAppManager.ProcessConstraints(migrationBuilder, constraints, schema, tableName);
//                }
//            }
//        }
//        public static void ProcessRowVersion(this MigrationBuilder migrationBuilder, string schema, string tableName)
//        {
//            migrationBuilder.Sql(
//                                $"ALTER TABLE {schema}.{tableName} ADD CONSTRAINT DF_{schema}_{tableName}_RowVersionAt DEFAULT GETDATE() FOR RowVersionAt;");
//            migrationBuilder.Sql(
//                $"ALTER TABLE {schema}.{tableName} ADD CONSTRAINT DF_{schema}_{tableName}_RowVersionBy DEFAULT SUSER_SNAME() FOR RowVersionBy;");
//            migrationBuilder.Sql($"ALTER TABLE {schema}.{tableName} ADD CONSTRAINT CK_{schema}_{tableName}_RowVersionBy CHECK(RowVersionBy NOT LIKE '%[^a-z!.!-!_!\\!@]%' ESCAPE '!');");
//        }

//        public static void ProcessConstraints(this MigrationBuilder migrationBuilder, Constraint[] constraints,  string schema, string tableName)
//        {
//            //var constraints = (Constraint[])annotation.Value;
//            foreach (var c in constraints)
//            {
//                var s = $"ALTER TABLE {schema}.{tableName} ADD CONSTRAINT {c.Name} {c.Body};";
//                migrationBuilder.Sql(s);
//            }
//        }
//    }
//}
