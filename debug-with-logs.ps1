# Script to run the application and capture logs

$logFile = "app_debug.log"

try {
    # Remove old log file if it exists
    if (Test-Path $logFile) {
        Remove-Item $logFile
    }

    Write-Host "Building application..." -ForegroundColor Cyan
    Set-Location -Path "IHECBookzone.Desktop"
    dotnet build 2>&1 | Tee-Object -FilePath "..\$logFile" -Append
    
    Write-Host "Starting application and logging output..." -ForegroundColor Green
    
    # Run the app and redirect all output to log file
    $process = Start-Process -FilePath "bin\Debug\net8.0-windows\IHECBookzone.Desktop.exe" -PassThru -Wait -NoNewWindow -RedirectStandardOutput "..\$logFile.stdout" -RedirectStandardError "..\$logFile.stderr"
    
    # Return to original directory
    Set-Location -Path ".."
    
    # Check process exit code
    Write-Host "Application exited with code: $($process.ExitCode)" -ForegroundColor Yellow
    
    # Add Windows event logs for the application
    Write-Host "Checking Windows Event Logs for errors..." -ForegroundColor Cyan
    Get-EventLog -LogName Application -EntryType Error -Newest 10 | 
        Where-Object { $_.TimeGenerated -gt (Get-Date).AddMinutes(-5) } | 
        Format-List -Property TimeGenerated, Source, Message | 
        Out-File -FilePath "$logFile.eventlog" -Append
        
    # Display all collected logs
    Write-Host "`nLOG OUTPUT:" -ForegroundColor Green
    if (Test-Path "$logFile.stdout") { 
        Write-Host "`nSTANDARD OUTPUT:" -ForegroundColor Cyan
        Get-Content "$logFile.stdout" 
    }
    
    if (Test-Path "$logFile.stderr") { 
        Write-Host "`nSTANDARD ERROR:" -ForegroundColor Red
        Get-Content "$logFile.stderr" 
    }
    
    if (Test-Path "$logFile.eventlog") { 
        Write-Host "`nEVENT LOG:" -ForegroundColor Yellow
        Get-Content "$logFile.eventlog" 
    }
}
catch {
    Write-Host "Error running application: $_" -ForegroundColor Red
    $_ | Out-File -FilePath $logFile -Append
}

Write-Host "`nAll logs saved to: $logFile*" -ForegroundColor Green
Write-Host "Press any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown") 