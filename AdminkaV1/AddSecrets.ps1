# Run this script to override a connectionString with "VS Secrets" feature and setup the secret locally  
dotnet user-secrets set "ConnectionStrings:DshbXConnection" "Data Source=(local); Initial Catalog=DshbX; Integrated Security=SSPI; Encrypt=False;" --id "ConnectionsStorage.EfCore.SqlServer"

dotnet user-secrets set "ClientSecret" "..." --id "ConnectionsStorage.AzureClientId"
