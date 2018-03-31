$dotnetPath = 'C:\Program Files\dotnet\dotnet.exe'

$SolutionFolderPath = $PSScriptRoot #or enter it manually there 
If ($SolutionFolderPath -eq '') {
    $SolutionFolderPath = 'D:\cot\DashboardCode'
    # throw "Rut it as script from the VS solution's root folder, this will point the location of the solution."
}
$BenchmarkReportPath = "$SolutionFolderPath\BenchmarkDotNet.Artifacts\results"
$BenchmarkProjectPath = "$SolutionFolderPath\Tests\Benchmark"

# this recompiles all target versions. dotnet run is not enough for it. but I'm not sure that this is obligated.
# NOTE: script doesn't work after project's Clear - it is still need to compile all from VS at first time. 
& $dotnetPath build $BenchmarkProjectPath -c Release

$snapshotBefore = Get-ChildItem *.html -path "$BenchmarkReportPath"

& dotnet run -c Release -f net47 -p "$BenchmarkProjectPath"

$snapshotAfter = Get-ChildItem *.html -path "$BenchmarkReportPath"
if ($snapshotBefore -eq $null -and $snapshotAfter -ne $null){
     Invoke-Item "BenchmarkReportPath\*.html"
 }else{
     $list = Compare-Object -ReferenceObject $snapshotBefore -DifferenceObject $snapshotAfter
     if ($list -ne $null){
         foreach ($c in $list) {
             $c.InputObject.FullName
             Invoke-Item $c.InputObject.FullName
         }
     }
}