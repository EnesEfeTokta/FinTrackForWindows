#!/bin/bash

# FinTrack Project Setup Script
# This script helps set up the development environment for FinTrack

echo "=== FinTrack Project Setup ==="
echo "Setting up FinTrack for Windows development environment..."

# Check if .NET 8 SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 8.0 SDK"
    echo "   Download from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

echo "✅ .NET SDK found: $(dotnet --version)"

# Navigate to project directory
cd "$(dirname "$0")"

# Restore packages
echo "📦 Restoring NuGet packages..."
dotnet restore FinTrack/FinTrackForWindows.csproj

# Check if we can build (note: will fail on non-Windows due to WPF)
echo "🔨 Attempting to build project..."
if dotnet build FinTrack/FinTrackForWindows.csproj --configuration Debug; then
    echo "✅ Project built successfully!"
else
    echo "⚠️  Build failed - this is expected on non-Windows platforms due to WPF dependencies"
    echo "   This project requires Windows with Visual Studio 2022 for full development"
fi

echo ""
echo "=== Project Information ==="
echo "Project Name: FinTrack for Windows"
echo "Framework: .NET 8.0"
echo "UI Framework: WPF (Windows Presentation Foundation)"
echo "Pattern: MVVM (Model-View-ViewModel)"
echo "Database: SQLite with Entity Framework Core"
echo ""
echo "=== Development Requirements ==="
echo "- Windows 10/11"
echo "- Visual Studio 2022 with WPF workload"
echo "- .NET 8.0 SDK"
echo ""
echo "=== Getting Started ==="
echo "1. Open FinTrackForWindows.sln in Visual Studio 2022"
echo "2. Build and run the project (F5)"
echo "3. The application will create a local SQLite database"
echo ""
echo "Project setup complete! 🎉"