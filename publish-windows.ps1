[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$project = Join-Path $root "MusiqueHockey\MusiqueHockey\MusiqueHockey.csproj"
$output = Join-Path $root "dist\windows-x64"

if (Test-Path $output) {
    Remove-Item $output -Recurse -Force
}

Write-Host "Publication d'Aréna DJ 2.0..." -ForegroundColor Cyan
dotnet publish $project `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    --output $output `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true

if ($LASTEXITCODE -ne 0) {
    throw "La publication a échoué (code $LASTEXITCODE)."
}

$executable = Join-Path $output "MusiqueHockey.exe"
if (-not (Test-Path $executable)) {
    throw "L'exécutable attendu n'a pas été créé : $executable"
}

Write-Host "`nNouvel exécutable créé :" -ForegroundColor Green
Write-Host $executable
Write-Host "Mettez à jour votre raccourci pour qu'il pointe vers ce fichier." -ForegroundColor Yellow
