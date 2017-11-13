Invoke-WebRequest https://chocolatey.org/install.ps1 -UseBasicParsing | Invoke-Expression

choco install nuget.commandline -y

choco list -li

get-command NuGet.exe

