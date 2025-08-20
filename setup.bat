@echo off
REM FinTrack Project Setup Script for Windows
REM This script helps set up the development environment for FinTrack

echo === FinTrack Project Setup ===
echo Setting up FinTrack for Windows development environment...

REM Check if .NET 8 SDK is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ .NET SDK not found. Please install .NET 8.0 SDK
    echo    Download from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo ✅ .NET SDK found
dotnet --version

REM Navigate to project directory
cd /d "%~dp0"

REM Restore packages
echo 📦 Restoring NuGet packages...
dotnet restore FinTrack\FinTrackForWindows.csproj

REM Build the project
echo 🔨 Building project...
dotnet build FinTrack\FinTrackForWindows.csproj --configuration Debug
if errorlevel 1 (
    echo ❌ Build failed. Please check the error messages above.
    pause
    exit /b 1
)

echo ✅ Project built successfully!

echo.
echo === Project Information ===
echo Project Name: FinTrack for Windows
echo Framework: .NET 8.0
echo UI Framework: WPF (Windows Presentation Foundation)
echo Pattern: MVVM (Model-View-ViewModel)
echo Database: SQLite with Entity Framework Core
echo.
echo === Development Requirements ===
echo - Windows 10/11
echo - Visual Studio 2022 with WPF workload
echo - .NET 8.0 SDK
echo.
echo === Getting Started ===
echo 1. Open FinTrackForWindows.sln in Visual Studio 2022
echo 2. Build and run the project (F5)
echo 3. The application will create a local SQLite database
echo.
echo Project setup complete! 🎉
pause