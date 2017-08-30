# Generate code coverage report using OpenCore and ReportGenerator. Home: https://github.com/rpokrovskij/opencover4vs.ps1 
# Put this script to Visual Studio solution folder. 

# You may need to run this from VS "Package Manager Console" on one of your test projects:
# nuget install OpenCover -OutputDirectory packages
# nuget install ReportGenerator -OutputDirectory packages
# nuget install coveralls.net -OutputDirectory packages     # optional

# CONFIGURATION
$TestProjectsGlobbing = @(,'*.Test.csproj')
$mstestPath = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\MSTest.exe' 
$dotnetPath = 'C:\Program Files\dotnet\dotnet.exe'
$netcoreapp = 'netcoreapp2.0'

$NamespaceInclusiveFilters = @(,'*') # asterix means inlude all namespaces (which pdb found)
$BuildNamespaceExclusiveFilters = $true # For core - test project's default namespace; For classic - namespaces where test project's types defined

$testClassicProjects=$true
$testCoreProjects   =$true
$debugMode          =$false

$toolsFolder = 'packages'
$classicProjectOutput = "bin\Debug"
$coreProjectOutput = "bin\Debug\$netcoreapp"

$testsResultsFolder = 'TestResults'

$excludeGlobbingFromFolders =  @('.git', '.vs', 'docs', $toolsFolder, $testsResultsFolder)

# left it empty if you are not using coveralls (publish report online, integrate it with GitHub. more https://coveralls.io/)
$env:COVERALLS_REPO_TOKEN = "ZD4n81FTV3l3GG5XTLmJszvEmD5DKyHo8" 

# STEP 1. Get Solution Folder
$SolutionFolderPath = $PSScriptRoot #or enter it manually there 

If ($SolutionFolderPath -eq '') {
    $SolutionFolderPath = 'D:\cot\DashboardCode'
    # throw "Rut it as script from the VS solution's root folder, this will point the location of the solution."
}

# STEP 2. Get OpenCover, ReportGenerator, Coveralls pathes
$openCoverPath       = Get-ChildItem -Path "$SolutionFolderPath\$toolsFolder" -Filter 'Opencover*'       -Directory | % { "$($_.FullName)\tools\OpenCover.Console.exe" }
$reportGeneratorPath = Get-ChildItem -Path "$SolutionFolderPath\$toolsFolder" -Filter 'ReportGenerator*' -Directory | % { "$($_.FullName)\tools\ReportGenerator.exe"   }
$coverallsPath       = Get-ChildItem -Path "$SolutionFolderPath\$toolsFolder" -Filter 'coveralls.net.*'  -Directory | % { "$($_.FullName)\tools\csmacnz.Coveralls.exe" }

# STEP 3. create TestResults folder
$testsResultsFolderPath = "$SolutionFolderPath\$testsResultsFolder"
If (Test-Path "$testsResultsFolderPath") { Remove-Item "$testsResultsFolderPath" -Recurse}
New-Item -ItemType Directory -Force -Path $testsResultsFolderPath | Out-Null

$openCoverOutputFilePath         = "$testsResultsFolderPath\opencoverOutput.xml"
$reportGeneratorOutputFolderPath = "$testsResultsFolderPath\report"


# STEP 5. find projects
$ClassicProjects =  @();
$CoreProjects = @();
Get-ChildItem "$SolutionFolderPath" -Directory -Exclude $excludeGlobbingFromFolders | %{ 
    Get-ChildItem  $_ -Recurse | %{
       foreach($i in $TestProjectsGlobbing){
          If ($_.FullName -ilike $i){
              $projFolder = $_.Directory.FullName
              $sdk = Select-XML -path $_.FullName -xpath "/*[local-name() = 'Project']/@Sdk"
              $assemblyNameProject = Select-XML -path $_.FullName -xpath "/*[local-name() = 'Project']/*[local-name() = 'PropertyGroup']/*[local-name() = 'AssemblyName']"
              $assemblyName = If (!$assemblyNameProject.Node.InnerText) { $_.BaseName  } Else { $assemblyNameProject.Node.InnerText}
              If ($sdk.Node.Value -eq "Microsoft.NET.Sdk"){
                 $assemblyPath = "$projFolder\$coreProjectOutput\$assemblyName.dll"
                 $CoreProjects += , @($_.FullName, $assemblyPath);
                 # TODO: Check that test csproj contains "/PropertyGroup/DebugType/text()=full" and "/PropertyGroup/DebugSymbols/text()=True"
              }
              Else{
                 $assemblyPath = "$projFolder\$classicProjectOutput\$assemblyName.dll"
                 $ClassicProjects+= , @($_.FullName, $assemblyPath);
              }
          }
       }
    }
}

Function GetNamespaces($dll) { 
     # run in separate process to do not lock solution's assemblies
     $namespaces = PowerShell -NoProfile -NonInteractive -Command {
         param([parameter(Position=0)][String]$dll) #,ValueFromPipeline=$True
         #write-host $dll+"bbb"
         $assembly = [System.Reflection.Assembly]::LoadFrom("$dll")
         $types = @()
         Try
         {
            $types = $assembly.GetTypes()
         }
         Catch [System.Reflection.ReflectionTypeLoadException] # Just in case. I have not met such situations, except with core dll's, but then .Types list contains only null values, so catch doesn't help.
         {
             $types = $_.Exception.Types | Where-Object {$_ -ne $Null} 
         }
         $namespaces = $types | foreach {$_.Namespace} | select-object -unique 
         return $namespaces;
     } -args "$dll" | foreach{ $_}
     return $namespaces;
}

Function GetFilter($inclusive, $exclusive) { 
     $filters = "" 
     foreach ($i in $inclusive) {
         $filters +="+[$i]* "
     }
     foreach ($i in $exclusive) {
         $filters +="-[$i]* "
     }
     return $filters;
}

#STEP 7. Execute OpenCover
If ($testClassicProjects){
    $mstestOutputFolderPath = "$testsResultsFolderPath\mstestOutput"
    New-Item -ItemType Directory -Force -Path $mstestOutputFolderPath | Out-Null
    If($debugMode){
        $trx=1
        Foreach($j in $ClassicProjects){
            $testDll = $j[1]
            $namespaces = @()
            if ($BuildNamespaceExclusiveFilters){
                $namespaces = GetNamespaces -dll $testDll
            }
            $filters = GetFilter -inclusive  $NamespaceInclusiveFilters -exclusive $namespaces
            $targetdir = Split-Path $testDll -parent
            $fileName  = Split-Path $testDll -leaf
            $targetargs="/testcontainer:$testDll /nologo /usestderr /resultsfile:$mstestOutputFolderPath\$fileName.$trx.trx"
            $trx+=1;
            echo "opencover -mergeoutput -register:user -mergebyhash -skipautoprops -target:$mstestPath -targetargs:$targetargs -filter:$filters -output:$openCoverOutputFilePath"
            & $openCoverPath -mergeoutput -register:user -mergebyhash -skipautoprops "-target:$mstestPath" "-targetargs:$targetargs" "-filter:$filters" "-output:$openCoverOutputFilePath"  
        }
    }
    Else{
        $targetargs = ""
        $namespaces = @();
        $trx=1;
        Foreach ($i in $ClassicProjects) {
             $testDll = $i[1]
             if ($BuildNamespaceExclusiveFilters){
                $namespaces += GetNamespaces -dll $testDll
             }
             $targetargs+= "/testcontainer:$testDll "
             $trx+=1;
        }
        $targetargs+=" /resultsfile:$mstestOutputFolderPath\multiple.$trx.trx"
        $namespaces = $namespaces | select-object -unique 
        $filters = GetFilter -inclusive  $NamespaceInclusiveFilters -exclusive $namespaces
        echo "opencover -register:user -target:$mstestPath -targetargs:$targetargs -filter:$filters -mergebyhash -skipautoprops -output:$openCoverOutputFilePath"
        & $openCoverPath "-register:user" "-target:$mstestPath" "-targetargs:$targetargs" "-filter:$filters" "-mergebyhash"  "-skipautoprops" "-output:$openCoverOutputFilePath"
    }
}

If ($testCoreProjects){
    Foreach($j in $CoreProjects){
        $testDll = $j[1]
        $projFilePath = $j[0]
        $rootNamespace = Select-XML -path $projFilePath -xpath "/*[local-name() = 'Project']/*[local-name() = 'PropertyGroup']/*[local-name() = 'RootNamespace']"
        $namespaces = @()
        If ($BuildNamespaceExclusiveFilters){
            $namespaces = (, $rootNamespace)
        }
        $filters = GetFilter -in  $NamespaceInclusiveFilters -out $namespaces
        # TODO: parsing the project file we can get TargetFramework 
        $targetargs = "test --no-build -f $netcoreapp -c Debug --verbosity normal $projFilePath"

        echo "opencover -oldStyle -mergeoutput -register:user -mergebyhash -skipautoprops -target:$dotnetPath -targetargs:$targetargs -filter:$filters -output:$openCoverOutputFilePath"
        & $openCoverPath -oldStyle -mergeoutput -register:user -mergebyhash -skipautoprops "-target:$dotnetPath" "-targetargs:$targetargs" "-filter:$filters" "-output:$openCoverOutputFilePath" 
    }
}

# STEP 8. Execute ReportGenerator

& $reportGeneratorPath "-reports:$openCoverOutputFilePath" "-targetdir:$reportGeneratorOutputFolderPath"

If ( Test-Path env:COVERALLS_REPO_TOKEN) { 
    If ($env:COVERALLS_REPO_TOKEN -ne "") {
        & $coverallsPath --opencover -i $openCoverOutputFilePath
    }
}

# STEP 9. Open report in a browser
If (Test-Path "$reportGeneratorOutputFolderPath\index.htm"){
    Invoke-Item "$reportGeneratorOutputFolderPath\index.htm"
}

# TODO: integrate with https://www.appveyor.com/