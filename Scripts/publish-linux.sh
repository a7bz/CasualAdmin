#!/bin/bash

# Multi-platform publishing script for CasualAdmin API - Linux

# Get the directory where this script is located
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
# Get the parent directory (project root)
PROJECT_ROOT="$SCRIPT_DIR/.."

# Show help if help requested
show_help() {
    echo "Usage: $0 [win|linux]"
    echo "  win   - Publish for Windows from Linux"
    echo "  linux - Publish for Linux (default)"
    exit 0
}

# Check arguments
case "$1" in
    "win")
        echo "=== Publishing CasualAdmin API (Windows from Linux) ==="
        PUBLISH_PATH="$PROJECT_ROOT/publish/win-x64"
        RUNTIME="win-x64"
        ;;
    "linux")
        echo "=== Publishing CasualAdmin API (Linux) ==="
        PUBLISH_PATH="$PROJECT_ROOT/publish/linux-x64"
        RUNTIME="linux-x64"
        ;;
    "")
        echo "=== Publishing CasualAdmin API (Linux) ==="
        PUBLISH_PATH="$PROJECT_ROOT/publish/linux-x64"
        RUNTIME="linux-x64"
        ;;
    *)
        show_help
        ;;
esac

# Clean old publish folder
if [ -d "$PUBLISH_PATH" ]; then
    echo "Cleaning old publish folder..."
    rm -rf "$PUBLISH_PATH"
fi

mkdir -p "$PUBLISH_PATH"

# Build solution
echo "Building solution..."
dotnet build "$PROJECT_ROOT/CasualAdmin.sln" -c Release

if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

# Publish API project
echo "Publishing API project for $RUNTIME..."
dotnet publish "$PROJECT_ROOT/CasualAdmin.API" -c Release -o "$PUBLISH_PATH" -r "$RUNTIME" --self-contained false

if [ $? -ne 0 ]; then
    echo "Publish failed!"
    exit 1
fi

# Make executable for the target platform
if [ "$RUNTIME" = "linux-x64" ]; then
    chmod +x "$PUBLISH_PATH/CasualAdmin.API" 2>/dev/null || true
elif [ "$RUNTIME" = "win-x64" ]; then
    echo "Windows executables don't need chmod"
fi

echo "=== Publish completed! ==="
echo "Publish path: $PUBLISH_PATH"
echo "Run with: dotnet CasualAdmin.API.dll"
