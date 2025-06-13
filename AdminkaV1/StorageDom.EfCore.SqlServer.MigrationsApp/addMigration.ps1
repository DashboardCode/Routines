# NOTE: run this to test verious issues 
# dotnet --version
# dotnet ef --help
# dotnet tool install --global dotnet-ef
# dotnet tool update --global dotnet-ef
# dotnet tool list -g
# dotnet ef --help
# dotnet ef --help migrations

# set migration name before creating migration
$MigrationName = "ExcConnectionIsActive2"
dotnet ef migrations add $MigrationName --project ../ConnectionsStorage.EfCore.SqlServer --context SqlServerExcDbContext 

# NOTE 1 do not use `dotnet ef database update` instead run the ConnectionsStorage.EfCore.SqlServer.MigrationsApp
# it has additional functionality 

# NOTE 2 to remove
# dotnet ef migrations remove ExcConnectionIsActive

# NOTE 3 it is impossible to create migrations in migrationApp project: when migrations created correctly the snapshot still created in the DbContext folder
# dotnet ef migrations add ExcConnectionIsActive --project ../ConnectionsStorage.EfCore.SqlServer --output-dir ../ConnectionsStorage.EfCore.SqlServer.MigrationsApp/Migrations --startup-project ../ConnectionsStorage.EfCore.SqlServer.MigrationsApp

# NOTE 4 afer adding non-nullable column default value should be edited manually
# e.g
# migrationBuilder.AddColumn<bool>(
#	name: "ExcConnectionIsActive",
#	table: "ExcConnections",
#	type: "bit",
#	nullable: false,
#	defaultValue: 1);