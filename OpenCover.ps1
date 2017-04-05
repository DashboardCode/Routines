# Generate code coverage report using OpenCore and ReportGenerator
# Put this file to Visual Studio solution folder. 

# CONFIGURATION
$mstestLocation = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\MSTest.exe' 
$TestDllsPatterns = @(,'*\bin\Debug\Vse*.Test.dll')  # use prefix to filter out xUnit Core tests (since they are from bin\Debug\netcore*). 
$TestableCodeNamespacePatterns = @(,'*') 

# STEP 0. Get Solution Folder
$SolutionFolder = $PSScriptRoot #or enter it manually there 

if ($SolutionFolder -eq ''){
    throw "Rut it as script from the VS solution's root folder, this will point the location of the solution."
}


# STEP 1. Get OpenCover path
$openCoverFolder = Get-ChildItem -Path "$SolutionFolder\packages" -Filter 'Opencover*' | Where-Object { $_.Attributes -eq "Directory"}
$openCoverLocation = "$SolutionFolder\packages\$openCoverFolder\tools\OpenCover.Console.exe"

# STEP 2. Get ReportGenerator path
$reportGeneratorFolder = Get-ChildItem -Path "$SolutionFolder\packages" -Filter 'ReportGenerator*' | Where-Object { $_.Attributes -eq "Directory"}
$reportGeneratorLocation = "$SolutionFolder\packages\$reportGeneratorFolder\tools\ReportGenerator.exe"

# STEP 3. create TestResults folder
$TestResultsFolder = $SolutionFolder+'\TestResults'
If (!(Test-Path "$TestResultsFolder")){
    New-Item -ItemType Directory -Force -Path $TestResultsFolder
}

# STEP 4. empty TestResults folder content if needed
$MsTestFolder = $TestResultsFolder+'\mstest'
#$TrxFile = "$MsTestFolder\output.trx"
$GeneratedReportsHtmlFolder = $TestResultsFolder+'\report'
$OpenCoverOutput ="$TestResultsFolder\opencover.xml"
If (Test-Path "$MsTestFolder"){
	Remove-Item "$MsTestFolder" -Recurse
}
If (Test-Path "$GeneratedReportsHtmlFolder"){
	Remove-Item "$GeneratedReportsHtmlFolder" -Recurse
}
If (Test-Path "$OpenCoverOutput"){
	Remove-Item "$OpenCoverOutput" 
}

#STEP 5. create folder for mstest results
If (!(Test-Path "$MsTestFolder")){
    New-Item -ItemType Directory -Force -Path $MsTestFolder | Out-Null
}

#STEP 6. Get Files
$TestDlls = @();
$excludes =  @('.git', '.vs', 'packages', 'TestResults', 'docs')
Get-ChildItem "$SolutionFolder" -Directory -Exclude $excludes | %{ 
    Get-ChildItem  $_ -Recurse | %{
       foreach($i in $TestDllsPatterns){
          if ($_.FullName  -ilike $i)
          {
              $TestDlls+=$_.FullName
          }
       }
    }
}
$TestDlls = $TestDlls | select-object -unique 

#STEP 7. Execute
$openCoverInOneLineForDebug=$true
if($openCoverInOneLineForDebug)
{
    $targetargs = ""
    $namespaces = @();
    $trx=1;
    foreach ($testDll in $TestDlls) {
         $assembly = [System.Reflection.Assembly]::LoadFrom("$testDll")
         $namespaces+= $assembly.GetTypes() | foreach {$_.Namespace} | select-object -unique 
         $targetargs+= "/testcontainer:$testDll "
         $trx+=1;
    }
    $targetargs+=" /resultsfile:$MsTestFolder\multiple.$trx.trx"
    $namespaces = $namespaces | select-object -unique 
    $filters = "" 
    foreach ($i in $TestableCodeNamespacePatterns) {
        $filters +="+[$i]* "
    }
    foreach ($i in $namespaces) {
        $filters +="-[$i]* "
    }
    
    & $openCoverLocation "-register:user" "-target:$mstestLocation" "-targetargs:$targetargs" "-filter:$filters" "-mergebyhash"  "-skipautoprops" "-output:$OpenCoverOutput"
}
else # usefull for debugging
{
    Push-Location 
    $trx=1
    foreach($testDll in $TestDlls)
    {
        $namespaces = @();
        $targetdir = Split-Path $testDll -parent
        $fileName  = Split-Path $testDll -leaf
		$assembly = [System.Reflection.Assembly]::LoadFrom("$testDll")
        $namespaces+= $assembly.GetTypes() | foreach {$_.Namespace} | select-object -unique 
        $targetargs="/testcontainer:$testDll /nologo /usestderr /resultsfile:$MsTestFolder\$fileName.$trx.trx"
        $namespaces = $namespaces | select-object -unique 
        $filters = "" 
        foreach ($i in $TestableCodeNamespacePatterns) {
            $filters +="+[$i]* "
        }
        foreach ($i in $namespaces) {
            $filters +="-[$i]* "
        }
        $trx+=1;
        cd $targetdir
        # Get-Location
        & $openCoverLocation "-register:user" "-target:$mstestLocation" "-targetargs:$targetargs" "-targetdir:$targetdir" "-filter:$filters" "-mergebyhash"  "-skipautoprops" "-output:$OpenCoverOutput" "-mergeoutput" 
    }
    Pop-Location
}

#STEP 8. Execute ReportGenerator

& $reportGeneratorLocation "-reports:$OpenCoverOutput" "-targetdir:$GeneratedReportsHtmlFolder"

#STEP 9. Open report in a browser
If (Test-Path "$GeneratedReportsHtmlFolder\index.htm"){
  Invoke-Item "$GeneratedReportsHtmlFolder\index.htm"
}


