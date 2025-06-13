using AdminkaV1.StorageDom.EfCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AdminkaV1.StorageDom.WebApi
{
    public static class StaticTools
    {
        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.sqlserver.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables() // <-- THIS is critical for Azure
                .AddUserSecrets("ConnectionsStorage.EfCore.SqlServer")
                .AddUserSecrets("ConnectionsStorage.AzureClientId"); // override with personally configured connection string; see AddSecrets.ps1 script; 
            return builder.Build();
        }

#if DEVDEBUG // mockup JWT token for dev time
        public const string DevTimeJwtSecurityTokenIssuer = "DshbX-dev-time";
        public const string DevTimeJwtSecurityKey = "SuperSecretKey12345nakasd1231230234012312asdsdflskdjfqweposdfsdfmsdfsd";
        public static void ConfigureDevTimeJwtBarrierOptions(JwtBearerOptions options, string? audience) 
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = DevTimeJwtSecurityTokenIssuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DevTimeJwtSecurityKey))
            };
        }
#endif


#if SQLLITE
            static readonly Microsoft.Data.Sqlite.SqliteConnection sqliteConnection = new("DataSource=:memory:");
            internal static ExcDbContext CreateExcDbContext()
            {
                sqliteConnection.Open();
                var excDbContext = new ExcDbContext(new DbContextOptionsBuilder<ExcDbContext>()
                       .UseSqlite(sqliteConnection)
                       .Options);
                var wasCreated = excDbContext.Database.EnsureCreated();
                if (wasCreated)
                {
                    var store = new ExcConnectionsStore(CreateExcDbContext);
                    store.CreateData();
                }
                return excDbContext;
            }
#else
        public static ExcDbContext CreateExcDbContext()
        {
            var connectionString = GetConfiguration().GetConnectionString("DshbXConnection");
            var excDbContext = new ExcDbContext(new DbContextOptionsBuilder<ExcDbContext>()
              .UseSqlServer(connectionString)
              .Options);

            excDbContext.Database.OpenConnection();
            return excDbContext;
        }
#endif
        public static PerCallContainer GetContainer()
        {
            return new PerCallContainer();
        }
        public class PerCallContainer
        {
            public IExcConnectionsStore GetExcConnectionsStore()
            {
                return new ExcConnectionsStore(CreateExcDbContext);
            }

        }

        public static IEdmModel GetEdmModel()
        {
            // odata model is used for json serializations
            var builder = new ODataConventionModelBuilder();
            builder.EnableLowerCamelCase();

            builder.EntitySet<ExcConnection>("connections").EntityType
                .HasKey(p => p.ExcConnectionId);
            
            builder.EntitySet<ExcTable>("exctables").EntityType
                .HasKey(p => p.ExcTableId); 
            
            return builder.GetEdmModel();
        }   
    }
}
