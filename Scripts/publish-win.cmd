@echo off

rem Multi-platform publishing script for Windows
rem Get the directory where this script is located
set "ScriptDir=%~dp0"
rem Remove trailing backslash
set "ScriptDir=%ScriptDir:~0,-1%"
rem Get the parent directory (project root)
set "ProjectRoot=%ScriptDir%\.."

if "%1"=="" goto :publish_windows
if "%1"=="linux" goto :publish_linux
if "%1"=="win" goto :publish_windows

:show_help
echo Usage: publish-win.cmd [win^|linux]
echo   win   - Publish for Windows (default)
echo   linux - Publish for Linux from Windows
exit /b 0

:publish_windows
echo === Publishing CasualAdmin API (Windows) ===
set "PublishPath=%ProjectRoot%\publish\win-x64"
set "Runtime=win-x64"
goto :publish

:publish_linux
echo === Publishing CasualAdmin API (Linux from Windows) ===
set "PublishPath=%ProjectRoot%\publish\linux-x64"
set "Runtime=linux-x64"
goto :publish

:publish
rem Clean old publish folder
if exist "%PublishPath%" (
    echo Cleaning old publish folder...
    rmdir /s /q "%PublishPath%"
)
mkdir "%PublishPath%" 2>nul

rem Build solution
echo Building solution...
dotnet build "%ProjectRoot%\CasualAdmin.sln" -c Release
if errorlevel 1 (
    echo Build failed!
    exit /b 1
)

rem Publish API project
echo Publishing API project for %Runtime%...
dotnet publish "%ProjectRoot%\CasualAdmin.API" -c Release -o "%PublishPath%" -r %Runtime% --self-contained false
if errorlevel 1 (
    echo Publish failed!
    exit /b 1
)

echo === Publish completed! ===
echo Publish path: %PublishPath%
echo Run with: dotnet CasualAdmin.API.dll
