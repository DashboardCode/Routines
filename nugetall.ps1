# TODO: should I migrate to nuget (with it I still need change folder): 
# nuget push "Routines.Storage.Ef6\bin\Release\DashboardCode.Routines.Storage.Ef6.3.0.6.nupkg" 'password' -Source https://api.nuget.org/v3/index.json

# CONFIGURATION
$IncludePublicableProjectsGlobbing = @('Routines.csproj','Routines.*.csproj')
$ExcludePublicableProjectsGlobbing = @(,'Routines.*.csproj')
$dotnetPath = 'C:\Program Files\dotnet\dotnet.exe'

# STEP 1. Get Solution Folder
$SolutionFolderPath = $PSScriptRoot #or enter it manually there 


# STEP 2. find projects
$CoreProjects = @('Routines.Storage.EfCore.Relational.InMemory',
'Routines','Routines.ActiveDirectory','Routines.AspNetCore', 'Routines.Configuration.Classic', 'Routines.Configuration.Standard',
'Routines.Storage.EfCore', 'Routines.Storage.EfCore.Relational', 'Routines.Storage.EfCore.Relational.SqlServer', 
'Routines.Storage.SqlServer', 'Routines.Storage.SystemSqlServer','Routines.Storage.Ef6'
);


$ver = '3.0.5'
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

cd $SolutionFolderPath
ForEach ($name in $CoreProjects) {& dotnet build $name --configuration Release}



$sign = Read-Host 'Enter sign'


Function PushToNuget ($name)
{
    cd $SolutionFolderPath\$name\bin\Release
    nuget push "DashboardCode.$name.$ver.nupkg" $sign -Source https://api.nuget.org/v3/index.json
}

ForEach ($name in $CoreProjects) {PushToNuget("$name")}


