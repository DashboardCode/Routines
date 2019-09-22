# TODO: should I migrate to nuget (with it I still need change folder): 
# dotnet nuget push mycomponet.1.0.0.nupkg -k 0000000000000000000000000 -s https://api.nuget.org/v3/index.json

# CONFIGURATION
$IncludePublicableProjectsGlobbing = @('Routines.csproj','Routines.*.csproj')
$ExcludePublicableProjectsGlobbing = @(,'Routines.*.csproj')
$dotnetPath = 'C:\Program Files\dotnet\dotnet.exe'

# STEP 1. Get Solution Folder
$SolutionFolderPath = $PSScriptRoot #or enter it manually there 


# STEP 2. find projects
$CoreProjects = @('Routines','Routines.ActiveDirectory','Routines.AspNetCore', 'Routines.Configuration.Classic', 'Routines.Configuration.Standard',
'Routines.Storage.EfCore', 'Routines.Storage.EfCore.Relational', 'Routines.Storage.EfCore.Relational.SqlServer', 'Routines.Storage.SqlServer' );


$ver = '2.0.67'
Function UpdateVersion ($projectFile)
{
    Get-Content -path $projectFile | % { $_ `
    -replace '<AssemblyVersion>(.*)</AssemblyVersion>', "<AssemblyVersion>$ver.0</AssemblyVersion>" `
    -replace '<FileVersion>(.*)</FileVersion>', "<FileVersion>$ver.0</FileVersion>" `
    -replace '<Version>(.*)</Version>', "<Version>$ver</Version>" `
    } |  Out-File "$projectFile.tmp"
    Remove-Item -Path "$projectFile"
    Move-Item -Path "$projectFile.tmp" -Destination "$projectFile"
}

ForEach ($name in $CoreProjects) {UpdateVersion("$SolutionFolderPath\$name\$name.csproj")}

& dotnet build --configuration Release


$sign = Read-Host 'Enter sign'
$ver = '2.0.65'

Function PushToNuget ($projectFile)
{
    cd "$projectFile\Routines\bin\Release"
    nuget push "DashboardCode.$name.$ver.nupkg" $sign -Source https://api.nuget.org/v3/index.json
}

ForEach ($name in $CoreProjects) {PushToNuget("$name","$SolutionFolderPath\$name\$name.csproj")}


#cd $SolutionFolderPath\Routines\bin\Release
#nuget push DashboardCode.Routines.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines.AspNetCore\bin\Release
#nuget push DashboardCode.Routines.AspNetCore.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines.Configuration.Standard\bin\Release
#nuget push DashboardCode.Routines.Configuration.Standard.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines.Storage.SqlServer\bin\Release
#nuget push DashboardCode.Routines.Storage.SqlServer.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines.Storage.EfCore.Relational\bin\Release
#nuget push DashboardCode.Routines.Storage.EfCore.Relational.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines.Storage.EfCore.Relational.SqlServer\bin\Release
#nuget push DashboardCode.Routines.Storage.EfCore.Relational.SqlServer.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines.Storage.EfCore\bin\Release
#nuget push DashboardCode.Routines.Storage.EfCore.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines.ActiveDirectory\bin\Release
#nuget push DashboardCode.Routines.ActiveDirectory.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json

#cd $SolutionFolderPath\Routines.Configuration.Classic\bin\Release
#nuget push DashboardCode.Routines.Configuration.Classic.$ver.nupkg $sign -Source https://api.nuget.org/v3/index.json
