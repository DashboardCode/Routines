$SolutionFolderPath = $PSScriptRoot #or enter it manually there 
If ($SolutionFolderPath -eq '') {
    $SolutionFolderPath = 'D:\cot\Vse'
    #throw "Rut it as script from the VS solution's root folder, this will point the location of the solution."
}

$BenchmarkPath = "$SolutionFolderPath\Tests\BenchmarkClassic\bin\Release"
cd "$BenchmarkPath"

# $dotnetPath = 'C:\Program Files\dotnet\dotnet.exe'
# & $dotnetPath "$SolutionFolderPath\Tests\Benchmark\bin\Debug\netcoreapp1.1\Benchmark.dll"
& "$BenchmarkPath\BenchmarkClassic.exe"

If (Test-Path "$BenchmarkPath\BenchmarkDotNet.Artifacts\results\*.html"){
    Invoke-Item "$BenchmarkPath\BenchmarkDotNet.Artifacts\results\*.html"
}