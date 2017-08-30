$dotnetPath = 'C:\Program Files\dotnet\dotnet.exe'

$SolutionFolderPath = $PSScriptRoot #or enter it manually there 
If ($SolutionFolderPath -eq '') {
    $SolutionFolderPath = 'D:\cot\DashboardCode'
    # throw "Rut it as script from the VS solution's root folder, this will point the location of the solution."
}

$BenchmarkProjectPath = "$SolutionFolderPath\Tests\BenchmarkClassic"

& $dotnetPath build $BenchmarkProjectPath -c Release

$BenchmarkProjectOutputPath = "$BenchmarkProjectPath\bin\Release"
$BenchmarkStartPath = $SolutionFolderPath
$BenchmarkReportPath = "$SolutionFolderPath\BenchmarkDotNet.Artifacts\results"
cd "$BenchmarkStartPath"

# $dotnetPath = 'C:\Program Files\dotnet\dotnet.exe'
# & $dotnetPath "$SolutionFolderPath\Tests\Benchmark\bin\Debug\netcoreapp1.1\Benchmark.dll"
$snap1 = Get-ChildItem *.html -path "$BenchmarkReportPath"

& "$BenchmarkProjectOutputPath\BenchmarkClassic.exe"

$snap2 = Get-ChildItem *.html -path "$BenchmarkReportPath"
if ($snap1 -eq $null -and $snap2 -ne $null){
	Invoke-Item "$SolutionFolderPath\BenchmarkDotNet.Artifacts\results\*.html"
}else{
    $list = Compare-Object -ReferenceObject $snap1 -DifferenceObject $snap2
    if ($list -ne $null){
        foreach ($c in $list) {
            $c.InputObject.FullName
            Invoke-Item $c.InputObject.FullName
        }
    }
}