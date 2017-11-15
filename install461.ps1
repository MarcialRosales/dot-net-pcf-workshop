Add-Type -AssemblyName System.IO.Compression.FileSystem
function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

Write-Host "`nCreating assemblies folder ...`n" -ForegroundColor Gray
New-Item -ItemType Directory -path "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework"

Write-Host "`nExtracting assemblies  ...`n" -ForegroundColor Gray
Unzip "assemblies\v.4.6.1.zip" "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework"

Write-Host "`nListing .net framework assemblies  ...`n" -ForegroundColor Gray
dir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework"
