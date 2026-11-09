<#
.SYNOPSIS
    Publishes the WinForms app (self-contained, win-x64) and builds the per-user MSI installer.

.DESCRIPTION
    Run from anywhere:   .\installer\build-installer.ps1
    Output:              artifacts\installer\XMLDifferenceTool-<version>-x64.msi

    The MSI version is read from the published exe (the same informational version the About box shows),
    keeping only the first three numeric parts: 1.2.3-beta+abc123 becomes 1.2.3.
    Change the version in Directory.Build.props (VersionPrefix), not here.

.PARAMETER ExpectedVersion
    Optional. The version the build must report, for example 1.2.3 or 1.2.3-beta.1 (the release workflow passes the
    version from the git tag). The script stops before building the MSI if the built app reports something else.

.PARAMETER AcceptWixEula
    Optional. Passes AcceptEula=wix7 to the WiX build, which records your acceptance of the WiX Toolset v7 EULA
    (https://docs.firegiant.com/wix/osmf/). The release workflow uses this; on your own PC you can accept the EULA
    once instead, as WiX explains.
#>
[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$ExpectedVersion,
    [switch]$AcceptWixEula
)

$ErrorActionPreference = 'Stop'

$repoRoot   = Split-Path -Parent $PSScriptRoot
$appProject = Join-Path $repoRoot 'src\XMLDifferenceTool\XMLDifferenceTool.csproj'
$wixProject = Join-Path $PSScriptRoot 'XMLDifferenceTool.Installer\XMLDifferenceTool.Installer.wixproj'
$publishDir = Join-Path $repoRoot 'artifacts\publish'
$outputDir  = Join-Path $repoRoot 'artifacts\installer'

# 1. Publish the app, self-contained for 64-bit Windows, into a clean folder
if (Test-Path $publishDir) { Remove-Item $publishDir -Recurse -Force }

Write-Host "Publishing the WinForms app..." -ForegroundColor Cyan
dotnet publish $appProject -c $Configuration -r win-x64 --self-contained true -o $publishDir
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed (exit code $LASTEXITCODE)." }

# 2. Work out the MSI version from the published exe
$exe = Join-Path $publishDir 'XMLDifferenceTool.exe'
if (-not (Test-Path $exe)) { throw "Expected $exe after publishing. Is the project's AssemblyName different?" }

$productVersion = (Get-Item $exe).VersionInfo.ProductVersion
if ($productVersion -notmatch '^(\d+)\.(\d+)\.(\d+)') {
    throw "Could not read a numeric x.y.z version from '$productVersion'."
}
$version = "$($Matches[1]).$($Matches[2]).$($Matches[3])"
Write-Host "Installer version: $version (from '$productVersion')" -ForegroundColor Cyan

# Guard against releasing with a version that does not match the git tag
$fullVersion = ($productVersion -split '\+')[0]
if ($ExpectedVersion -and $fullVersion -ne $ExpectedVersion) {
    throw "Version mismatch: expected '$ExpectedVersion' (from the tag) but the built app reports '$fullVersion'. Update VersionPrefix/VersionSuffix in Directory.Build.props, commit, and tag again."
}

# 3. Build the MSI
Write-Host "Building the MSI..." -ForegroundColor Cyan
$wixArgs = @('build', $wixProject, '-c', $Configuration, "-p:AppVersion=$version", "-p:PublishFolder=$publishDir")
if ($AcceptWixEula) { $wixArgs += '-p:AcceptEula=wix7' }
dotnet @wixArgs
if ($LASTEXITCODE -ne 0) { throw "WiX build failed (exit code $LASTEXITCODE)." }

# 4. Copy the newest MSI the build produced into artifacts\installer
$msi = Get-ChildItem (Join-Path $PSScriptRoot 'XMLDifferenceTool.Installer\bin') -Recurse -Filter *.msi |
       Sort-Object LastWriteTime -Descending |
       Select-Object -First 1
if (-not $msi) { throw "The build finished but no .msi file was found under the installer's bin folder." }

New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
Copy-Item $msi.FullName $outputDir -Force

Write-Host ""
Write-Host "Done: $(Join-Path $outputDir $msi.Name)" -ForegroundColor Green
