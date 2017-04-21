$SolutionFolderPath = $PSScriptRoot #or enter it manually there 
If ($SolutionFolderPath -eq '') {
    $SolutionFolderPath = 'D:\cot\Vse'
    #throw "Rut it as script from the VS solution's root folder, this will point the location of the solution."
}

$BenchmarkPath = "$SolutionFolderPath\Tests\Benchmark\bin\Debug\net462"
cd "$BenchmarkPath"

# $dotnetPath = 'C:\Program Files\dotnet\dotnet.exe'
# & $dotnetPath "$SolutionFolderPath\Tests\Benchmark\bin\Debug\netcoreapp1.1\Benchmark.dll"
& "$BenchmarkPath\Benchmark.exe"

If (Test-Path "$BenchmarkPath\BenchmarkDotNet.Artifacts\results\BenchmarkHashset-report.html"){
    Invoke-Item "$BenchmarkPath\BenchmarkDotNet.Artifacts\results\BenchmarkHashset-report.html"
}