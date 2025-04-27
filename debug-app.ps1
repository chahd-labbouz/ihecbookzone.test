# Script to debug IHECBookzone Desktop application

# Set environment variables for debugging
$env:DOTNET_CLI_UI_LANGUAGE = "en"
$env:DOTNET_CONSOLE_UI_LANGUAGE = "en"

# Enable WPF diagnostics logging
$env:WPF_VERBOSE_DIAGNOSTICS = "1"
$env:COMPlus_LogEnable = "1"
$env:COMPlus_LogFacility = "0xffffffff"
$env:COMPlus_LogLevel = "0"
$env:COMPlus_LogToConsole = "1"

# Navigate to project directory
Set-Location -Path "IHECBookzone.Desktop"

Write-Host "Building project in debug configuration..." -ForegroundColor Cyan
dotnet build -c Debug

Write-Host "Running application..." -ForegroundColor Green
dotnet run --no-build --environment="Development" --debug

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 