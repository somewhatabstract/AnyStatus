@echo off

rd /s /q %~dp0\OpenCover
md OpenCover

REM OpenCover
"%~dp0\src\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe" ^
-register:user ^
-target:"%VS140COMNTOOLS%\..\IDE\mstest.exe" ^
-targetargs:"/testcontainer:\"%~dp0\src\AnyStatus.Tests\bin\Debug\AnyStatus.Tests.dll\" /testcontainer:\"%~dp0\src\AnyStatus.Integration.Tests\bin\Debug\AnyStatus.Integration.Tests.dll\" /resultsfile:\"%~dp0\OpenCover\AnyStatus.trx\"" ^
-filter:"+[AnyStatus*]* -[AnyStatus.Tests]* -[AnyStatus.Integration.Tests]* -[FluentScheduler]*" ^
-mergebyhash ^
-skipautoprops ^
-excludebyattribute:*.ExcludeFromCodeCoverage* ^
-hideskipped:attribute ^
-output:"%~dp0\OpenCover\Coverage.xml"

REM Report Generator
"%~dp0\src\packages\ReportGenerator.2.5.1\tools\ReportGenerator.exe" ^
-reports:"%~dp0\OpenCover\Coverage.xml" ^
-targetdir:"%~dp0\OpenCover\Report"

start "" "%~dp0\OpenCover\Report\index.htm"