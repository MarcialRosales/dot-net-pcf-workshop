Invoke-WebRequest https://chocolatey.org/install.ps1 -UseBasicParsing | Invoke-Expression

choco install nuget.commandline -y

Write-Host "listing chocolatey packages" -ForegroundColor Green
choco list -li

Write-Host "getting where nuget.exe" -ForegroundColor Green
get-command NuGet.exe

