# Start React development server
Write-Host "Starting React development server..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd src/QuantumQueue.Client; npm start"

# Wait a moment for React to initialize
Start-Sleep -Seconds 5

# Start .NET API
Write-Host "Starting .NET API..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd src/OxCore.QuantumQueue.Api; dotnet run"

Write-Host "Both servers are starting up..." -ForegroundColor Yellow
Write-Host "React app will be available at: http://localhost:3000" -ForegroundColor Cyan
Write-Host "API will be available at: https://localhost:7001" -ForegroundColor Cyan 