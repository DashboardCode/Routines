// proposed by ChatGpt - doesn't work (old version) 
// TODO: Snapshot should be generated in this app (currently doens't work) or implement add-migration manually 
/*
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using ConnectionsStorage.EfCore;

namespace ConnectionsStorage.EfCore.SqlServer.MigrationsApp
{
    public class MigrationManager
    {
        public static void AddMigrationManager()
        {
            var migrationName = "MyManualMigration"; // Name of the migration to be created
            var outputDir = "Migrations";
            var @namespace = "MigrationGeneratorApp.Migrations";

            // Setup service provider
            var services = new ServiceCollection()
                //.AddLogging(builder => builder.AddConsole())
                .AddDbContext<ExcDbContext>(options =>
                    options.UseSqlServer("Your_Connection_String"))
                .AddEntityFrameworkDesignTimeServices()
                .BuildServiceProvider();

            var scaffolder = services.GetRequiredService<IMigrationsScaffolder>();

            var result = scaffolder.ScaffoldMigration(
                migrationName: migrationName,
                rootNamespace: @namespace,
                subNamespace: null,
                language: "C#",
                outputDir: outputDir
            );

            // Save to files
            scaffolder.Save(result);

            Console.WriteLine($"Migration '{migrationName}' created successfully.");
        }
    }
}
*/