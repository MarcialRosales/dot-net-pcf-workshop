Invoke-WebRequest https://chocolatey.org/install.ps1 -UseBasicParsing | Invoke-Expression

choco install microsoft-build-tools -y
choco install nuget.commandline -y

get-command nuget.exe
get-command msbuild.exe

