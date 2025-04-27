# Script to run IHECBookzone Desktop application with console output visible

Set-Location -Path "IHECBookzone.Desktop\bin\Debug\net8.0-windows"

try {
    Write-Host "Starting IHECBookzone Desktop application..." -ForegroundColor Green
    
    # This will capture any exceptions thrown when running the exe
    $process = Start-Process -FilePath ".\IHECBookzone.Desktop.exe" -PassThru -Wait

    if ($process.ExitCode -ne 0) {
        Write-Host "Application terminated with exit code: $($process.ExitCode)" -ForegroundColor Red
    }
}
catch {
    Write-Host "Error starting application: $_" -ForegroundColor Red
}
finally {
    Set-Location -Path "..\..\..\..\"
}

Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 