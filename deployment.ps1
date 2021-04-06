$websiteUrl = 'https://localhost:5001'
$instance = '.'
$dbName = 'MindedBanking'

# Run npm install
Write-Host "Restoring NPM packages ..." -ForegroundColor Green
Set-Location ./MBApplicationUI
npm install

# Build angular app and Deploy wwwroot
Write-Host "Building angular App and deploying to wwwroot ..." -ForegroundColor Green
ng build --output-path ..\MB.Application.Api\wwwroot\

# Addmin SQLServer module for powershell
If(-not(Get-InstalledModule SQLServer -ErrorAction silentlycontinue)){
    Write-Host "Installing SQLServer module for powershell ..." -ForegroundColor Yellow
    Install-Module SQLServer -Confirm:$False -Force -AllowClobber
}
Import-Module -Name "SqlServer"
Push-Location SQLSERVER:\SQL\$instance

# Create Database
# TODO check if DB already exist, if so skip
Write-Host "Creating database 'MindedBanking' ..." -ForegroundColor Green
$db = New-Object -TypeName Microsoft.SqlServer.Management.Smo.Database -Argumentlist $instance, $dbName
$db.Create()
$db.SetOwner('sa')
Pop-Location

# Update database
Write-Host "Applying EF migrations to update the database ..." -ForegroundColor Green
Set-Location ./../MB.Application.Api
dotnet ef database update

# Build and run .net app
Set-Location ./../
Write-Host "Running .net app ..." -ForegroundColor Green
Start-Process -FilePath 'dotnet' -WorkingDirectory './MB.Application.Api' -ArgumentList 'run'

function Get-WebStatus {
    try{
        return [int][System.Net.WebRequest]::Create($websiteUrl).GetResponse().StatusCode
    }catch {
        return 0;
    }
}

Write-Host "Waiting for the site to start ..." -ForegroundColor Green
# Let's give dotnet some time to start
$attemptsLeft = 10;
while(($attemptsLeft -gt 0) -And ((Get-WebStatus) -ne 200)) {
    $attemptsLeft--
    Write-Host "Waiting for the website to become available ... (Attempts left: $attemptsLeft)"
    Start-Sleep -Seconds 3
}

Write-Host "Launching the browser ..." -ForegroundColor Green
# Run .net app and open it
Start $websiteUrl