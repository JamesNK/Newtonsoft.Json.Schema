param(
  [Parameter(Mandatory = $true)]
  [string]$PackageName,

  [Parameter(Mandatory = $true)]
  [string]$PackageVersion,

  [Parameter(Mandatory = $true)]
  [string]$PackageSupplier,

  [Parameter(Mandatory = $true)]
  [string]$NamespaceUriBase,

  [Parameter(Mandatory = $true)]
  [string]$BuildDropPath,

  [Parameter(Mandatory = $true)]
  [string]$TempDirectory,

  [Parameter(Mandatory = $true)]
  [string]$SbomToolVersion
)

$ErrorActionPreference = "Stop"

$toolPath = Join-Path $TempDirectory "sbom-tool"
dotnet tool install Microsoft.Sbom.DotNetTool --tool-path $toolPath --version $SbomToolVersion
if ($LASTEXITCODE -ne 0) { throw "Failed to install Microsoft SBOM Tool." }

$sbomTool = Join-Path $toolPath "sbom-tool.exe"
$packagePath = Join-Path $BuildDropPath "$PackageName.$PackageVersion.nupkg"
$componentPath = Join-Path $TempDirectory "sbom-components"
$validationOutput = Join-Path $TempDirectory "sbom-validation.json"

# Use the packed nuspec as the source of truth so build-only dependencies are excluded.
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath)
try {
  $nuspecEntry = $archive.Entries | Where-Object FullName -Like "*.nuspec" | Select-Object -First 1
  if ($null -eq $nuspecEntry) { throw "NuGet package does not contain a nuspec." }
  $reader = New-Object System.IO.StreamReader($nuspecEntry.Open())
  try { [xml]$nuspec = $reader.ReadToEnd() } finally { $reader.Dispose() }
} finally { $archive.Dispose() }

$dependencies = @($nuspec.SelectNodes("//*[local-name()='dependency']")) |
  ForEach-Object { [pscustomobject]@{ Id = $_.id; Version = $_.version } } |
  Sort-Object Id, Version -Unique
if ($dependencies.Count -eq 0) { throw "NuGet package has no declared dependencies." }

New-Item -Path $componentPath -ItemType Directory -Force | Out-Null
$config = New-Object System.Xml.XmlDocument
$packages = $config.CreateElement("packages")
$config.AppendChild($packages) | Out-Null
foreach ($dependency in $dependencies) {
  $package = $config.CreateElement("package")
  $package.SetAttribute("id", $dependency.Id)
  $package.SetAttribute("version", $dependency.Version)
  $packages.AppendChild($package) | Out-Null
}
$config.Save((Join-Path $componentPath "packages.config"))

# The manifest is written under the build drop and retained with the published artifacts.
& $sbomTool generate -b $BuildDropPath -bc $componentPath -pn $PackageName -pv $PackageVersion -ps $PackageSupplier -nsb $NamespaceUriBase -mi SPDX:2.2
if ($LASTEXITCODE -ne 0) { throw "Failed to generate SBOM." }

& $sbomTool validate -b $BuildDropPath -o $validationOutput -mi SPDX:2.2 -n
if ($LASTEXITCODE -ne 0) { throw "Failed to validate SBOM." }

# File validation is separate from checking that all declared dependencies were detected.
$manifestPath = Join-Path $BuildDropPath "_manifest\spdx_2.2\manifest.spdx.json"
$manifest = Get-Content $manifestPath | Out-String | ConvertFrom-Json
$missingDependencies = @($dependencies | Where-Object { $_.Id -notin $manifest.packages.name })
if ($missingDependencies.Count -ne 0) { throw "SBOM is missing declared dependencies: $($missingDependencies.Id -join ', ')." }

Write-Host "Generated and validated SBOM at $manifestPath"