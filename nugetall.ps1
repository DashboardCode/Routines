# TODO: should I migrate to nuget (with it I still need change folder): 
# dotnet nuget push mycomponet.1.0.0.nupkg -k 0000000000000000000000000 -s https://api.nuget.org/v3/index.json

$SolutionFolderPath = $PSScriptRoot 
cd $SolutionFolderPath

$sign = Read-Host 'Enter sign'

#cd .\NLogTools\bin\Release
#nuget push DashboardCode.NLogTools.1.0.0.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines\bin\Release
#nuget push DashboardCode.Routines.2.0.3.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines.AspNetCore\bin\Release
#nuget push DashboardCode.Routines.AspNetCore.2.0.3.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Configuration.Standard\bin\Release
nuget push DashboardCode.Routines.Configuration.Standard.2.0.3.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Storage.SqlServer\bin\Release
nuget push DashboardCode.Routines.Storage.SqlServer.2.0.3.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Storage.EfCore.Relational\bin\Release
nuget push DashboardCode.Routines.Storage.EfCore.Relational.2.0.3.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Storage.EfCore\bin\Release
nuget push DashboardCode.Routines.Storage.EfCore.2.0.3.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.ActiveDirectory\bin\Release
nuget push DashboardCode.Routines.ActiveDirectory.2.0.3.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Configuration.Classic\bin\Release
nuget push DashboardCode.Routines.Configuration.Classic.2.0.3.nupkg $sign -Source https://api.nuget.org/v3/index.json
