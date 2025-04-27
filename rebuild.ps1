# Script to clean and rebuild the IHECBookzone Desktop application

Write-Host "Cleaning solution..." -ForegroundColor Cyan
Set-Location -Path "IHECBookzone.Desktop"
dotnet clean

Write-Host "Deleting bin and obj folders..." -ForegroundColor Cyan
if (Test-Path -Path "bin") {
    Remove-Item -Path "bin" -Recurse -Force
}
if (Test-Path -Path "obj") {
    Remove-Item -Path "obj" -Recurse -Force
}

Write-Host "Restoring packages..." -ForegroundColor Cyan
dotnet restore

Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! Starting application..." -ForegroundColor Green
    dotnet run
} else {
    Write-Host "Build failed. See errors above." -ForegroundColor Red
} 