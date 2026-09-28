@echo off
rem Builds WfmAlert.exe with the C# compiler that ships with Windows (no SDK needed).
rem Put this file next to WfmAlert_market_enhancements.cs and double-click it.
cd /d "%~dp0"

if not exist ReleaseAsset mkdir ReleaseAsset

set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe
if not exist "%CSC%" (
  echo csc.exe was not found. .NET Framework 4.x is required.
  pause
  exit /b 1
)

"%CSC%" /nologo /target:winexe /optimize+ /codepage:65001 /win32icon:WfmAlert.ico /out:ReleaseAsset\WfmAlert.exe /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll /r:System.Web.Extensions.dll /r:System.Security.dll WfmAlert_market_enhancements.cs
if errorlevel 1 (
  echo.
  echo Build failed. See the messages above.
  pause
  exit /b 1
)

echo.
echo Done: ReleaseAsset\WfmAlert.exe
pause
