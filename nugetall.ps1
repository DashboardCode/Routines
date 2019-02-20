# TODO: should I migrate to nuget (with it I still need change folder): 
# dotnet nuget push mycomponet.1.0.0.nupkg -k 0000000000000000000000000 -s https://api.nuget.org/v3/index.json

$SolutionFolderPath = $PSScriptRoot 
cd $SolutionFolderPath

$sign = Read-Host 'Enter sign'
$ver = '2.0.40'

cd $SolutionFolderPath\Routines\bin\Release
nuget push DashboardCode.Routines.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.AspNetCore\bin\Release
nuget push DashboardCode.Routines.AspNetCore.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Configuration.Standard\bin\Release
nuget push DashboardCode.Routines.Configuration.Standard.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Storage.SqlServer\bin\Release
nuget push DashboardCode.Routines.Storage.SqlServer.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Storage.EfCore.Relational\bin\Release
nuget push DashboardCode.Routines.Storage.EfCore.Relational.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Storage.EfCore.Relational.SqlServer\bin\Release
nuget push DashboardCode.Routines.Storage.EfCore.Relational.SqlServer.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Storage.EfCore\bin\Release
nuget push DashboardCode.Routines.Storage.EfCore.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.ActiveDirectory\bin\Release
nuget push DashboardCode.Routines.ActiveDirectory.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

cd $SolutionFolderPath\Routines.Configuration.Classic\bin\Release
nuget push DashboardCode.Routines.Configuration.Classic.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json
