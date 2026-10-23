[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$root = $PSScriptRoot
$project = Join-Path $root "MusiqueHockey\MusiqueHockey\MusiqueHockey.csproj"
$output = Join-Path $root "dist\windows-x64"

if (Test-Path $output) {
    Remove-Item $output -Recurse -Force
}

Write-Host "Publication Windows autonome d'Aréna DJ..." -ForegroundColor Cyan

# A framework-dependent or single-file build can fail on a clean PC because of
# missing runtimes or native extraction. Publish every runtime/dependency file
# explicitly so Inno Setup can install a completely self-contained application.
dotnet publish $project `
    --configuration Release `
    --runtime win-x64 `
    --self-contained true `
    --output $output `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false `
    -p:DebugType=None `
    -p:DebugSymbols=false

if ($LASTEXITCODE -ne 0) {
    throw "La publication a échoué (code $LASTEXITCODE)."
}

$requiredFiles = @(
    "MusiqueHockey.exe",
    "MusiqueHockey.dll",
    "MusiqueHockey.deps.json",
    "MusiqueHockey.runtimeconfig.json",
    "coreclr.dll",
    "hostfxr.dll",
    "NAudio.dll",
    "Npgsql.dll"
)

foreach ($relative in $requiredFiles) {
    $file = Join-Path $output $relative
    if (-not (Test-Path $file)) {
        throw "Fichier indispensable absent de la publication : $relative"
    }
}

# The application expects these sections to exist even on a brand-new PC.
$musicCategories = @("all", "entracte", "finGame", "PPLocal", "PPvis", "buts", "warmup")
foreach ($category in $musicCategories) {
    New-Item -ItemType Directory -Path (Join-Path $output "netX\musiques\$category") -Force | Out-Null
}

Write-Host ""
Write-Host "Publication complète créée :" -ForegroundColor Green
Write-Host $output
Write-Host "Runtime .NET, dépendances audio/base de données et structure musicale vérifiés." -ForegroundColor Green
