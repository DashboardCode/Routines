# nuget spec
.\nuget.exe pack Routines.1.0.9.nuspec

# delete all except App_Data folder and its content
# D:\cot\Vse\AdminkaV1\Web\bin\Debug\net461\win7-x64

# TOREAD: http://docs.nuget.org/ndocs/tools/nuget.exe-cli-reference#pack
#         http://docs.nuget.org/ndocs/create-packages/symbol-packages nuget package symbols    
 
# TODO: change assembly version
# rename Routines.nuspec to Routines.0.0.ver.nuspec and then delete it after Routines.0.0.ver.nupkg created;

# $path = "C:\temp\test.txt"
# $pattern = '\[assembly: AssemblyVersion\("(.*)"\)\]'
# (Get-Content $path) | ForEach-Object{
#     if($_ -match $pattern){
#         # We have found the matching line
#         # Edit the version number and put back.
#         $fileVersion = [version]$matches[1]
#         $newVersion = "{0}.{1}.{2}.{3}" -f $fileVersion.Major, $fileVersion.Minor, $fileVersion.Build, ($fileVersion.Revision + 1)
#         '[assembly: AssemblyVersion("{0}")]' -f $newVersion
#     } else {
#         # Output line as is
#         $_
#     }
# } | Set-Content $path


# TODO: Create libs referenced with various .net flavoour from .net45 till .net461
# Note, csproj doesn't have a strong reference to System assembly, it is a task for tooling to select the apporpiate System version
# So the solution could be:
# for .NET 4.5.2  
# "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" Routines.csproj /p:SolutionFile=PathToSolution\App.sln;NuspecFile=foo.nuspec
# and for .NET 4.6
# "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe"


# TODO: Publish. 
# sample pbublis (works with local NUGET server) from https://dzone.com/articles/using-powershell-publish-nuget

#$nugetServer = "https://<your nuget server here>"
#$apiKey = "<your api key here>"
#$packageName = "<your package name here>"

#$latestRelease = nuget list $packageName
#$version = $latestRelease.split(" ")[1];

#$versionTokens = $version.split(".")
#$buildNumber = [System.Double]::Parse($versionTokens[$versionTokens.Count -1]) 
#$versionTokens[$versionTokens.Count -1] = $buildNumber +1
#$newVersion = [string]::join('.', $versionTokens)
#echo $newVersion

#get-childitem | where {$_.extension -eq ".nupkg"} | foreach ($_) {remove-item $_.fullname}
#nuget pack -Version $newVersion
#$package = get-childitem | where {$_.extension -eq ".nupkg"}
#nuget push -Source $nugetServer $package $apiKey