REM install two Nuget packages first: OpenCover and ReportGenerator
echo "%~dp0"

REM Create a 'GeneratedReports' folder if it does not exist
if not exist "%~dp0TestResults\GeneratedReports" mkdir "%~dp0TestResults\GeneratedReports"
 
REM Remove any previous test execution files to prevent issues overwriting
IF EXIST "%~dp0TestResults\AdminkaV1.Injected.Test.trx" del "%~dp0TestResults\AdminkaV1.Injected.Test.trx%"
 
REM Remove any previously created test output directories
CD %~dp0
FOR /D /R %%X IN (%USERNAME%*) DO RD /S /Q "%%X"
 
REM Run the tests against the targeted output
call :RunOpenCoverUnitTestMetrics
 
REM Generate the report output based on the test results
if %errorlevel% equ 0 (
 call :RunReportGeneratorOutput
)
 
REM Launch the report
if %errorlevel% equ 0 (
 call :RunLaunchReport
)
exit /b %errorlevel%
 
:RunOpenCoverUnitTestMetrics
"%~dp0packages\Opencover.4.6.519\Tools\OpenCover.Console.exe" ^
-register:user ^
-target:"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\mstest.exe" ^
-targetargs:"/testcontainer:\"%~dp0Tests\AdminkaV1.Injected.Test\bin\Debug\Vse.AdminkaV1.Injected.Test.dll\" /testcontainer:\"%~dp0Tests\Routines.Test\bin\Debug\Vse.Routines.Test.dll\" /testcontainer:\"%~dp0Tests\Routines.Configuration.Test\bin\Debug\Vse.Routines.Configuration.Test.dll\" /testcontainer:\"%~dp0Tests\AdminkaV1.Wcf.Client.Test\bin\Debug\Vse.AdminkaV1.Wcf.Client.Test.dll\" /testcontainer:\"%~dp0Tests\Web.Test\bin\Debug\Vse.Web.Test.dll\" /resultsfile:\"%~dp0TestResults\AdminkaV1.Injected.Test.trx\"" ^
-mergebyhash ^
-skipautoprops ^
-output:"%~dp0TestResults\GeneratedReports\AdminkaV1.Injected.Test.xml"
exit /b %errorlevel%
 
:RunReportGeneratorOutput
"%~dp0packages\ReportGenerator.2.5.1\Tools\ReportGenerator.exe" ^
-reports:"%~dp0TestResults\GeneratedReports\AdminkaV1.Injected.Test.xml" ^
-targetdir:"%~dp0TestResults\GeneratedReports\ReportGenerator Output"
exit /b %errorlevel%
 
:RunLaunchReport
start "report" "%~dp0TestResults\GeneratedReports\ReportGenerator Output\index.htm"
exit /b %errorlevel%

