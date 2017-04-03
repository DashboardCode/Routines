#CONFIGURATION
$mstestLocation = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\mstest.exe' 
$TestDllsPatterns = @(,'*\bin\Debug\Vse.*.Test.dll')  
$TestableCodeNamespacePatterns = @(,'*') 

# STEP 0. Get Solution Folder
if ($PSScriptRoot -eq ''){
    throw "Rut it as script from the solution root folder, this will point the solution location."
}
$SolutionFolder = $PSScriptRoot #or enter it manually there

# STEP 1. Get OpenCover path
$openCoverFolder = Get-ChildItem -Path "$SolutionFolder\packages" -Filter 'Opencover*' | Where-Object { $_.Attributes -eq "Directory"}
$openCoverLocation = "$SolutionFolder\packages\$openCoverFolder\tools\OpenCover.Console.exe"

# STEP 2. Get OpenCover path
$reportGeneratorFolder = Get-ChildItem -Path "$SolutionFolder\packages" -Filter 'ReportGenerator*' | Where-Object { $_.Attributes -eq "Directory"}
$reportGeneratorLocation = "$SolutionFolder\packages\$reportGeneratorFolder\tools\ReportGenerator.exe"

# STEP 3. create TestResults folder
$TestResultsFolder = $SolutionFolder+'\TestResults'
If (!(Test-Path "$TestResultsFolder")){
    New-Item -ItemType Directory -Force -Path $TestResultsFolder
}

# STEP 4. empty TestResults folder content if needed
$MsTestFolder = $TestResultsFolder+'\mstest'
$TrxFile = "$MsTestFolder\ControlledSerializationJsonConverterTests.trx"
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
$targetargs = ""
$namespaces = @();
 foreach ($testDll in $TestDlls) {
     $assembly = [System.Reflection.Assembly]::LoadFrom("$testDll")
     $namespaces+= $assembly.GetTypes() | foreach {$_.Namespace} | select-object -unique 
     $targetargs+= "/testcontainer:$testDll "
}
$targetargs+=" /resultsfile:$TrxFile"
$namespaces = $namespaces | select-object -unique 
$filters = "" 
foreach ($i in $TestableCodeNamespacePatterns) {
    $filters +="+[$i]*"
}
foreach ($i in $namespaces) {
    $filters +=" -[$i]*"
}

& $openCoverLocation "-register:user" "-target:$mstestLocation" "-targetargs:$targetargs" "-filter:$filters" "-mergebyhash"  "-skipautoprops" "-output:$OpenCoverOutput"

& $reportGeneratorLocation "-reports:$OpenCoverOutput" "-targetdir:$GeneratedReportsHtmlFolder"

#STEP 8. Open report in a browser
If (Test-Path "$GeneratedReportsHtmlFolder\index.htm"){
   Invoke-Item "$GeneratedReportsHtmlFolder\index.htm"
}