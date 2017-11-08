Invoke-WebRequest https://chocolatey.org/install.ps1 -UseBasicParsing | Invoke-Expression

choco install nuget.commandline -y

get-command nuget.exe

nuget.exe
