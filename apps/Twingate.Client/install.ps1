# Twingate ships a single x64 MSI whose CheckNETRuntime custom action requires the
# x64 .NET 8 Desktop Runtime. On ARM64 the arm64 runtime satisfies winget's dependency
# check but not the MSI's, so the install fails with 1603 / CheckNETRuntime_Result = 12290.
$x64DesktopRuntime = Join-Path $env:ProgramFiles "dotnet\x64\shared\Microsoft.WindowsDesktop.App"
if ($env:PROCESSOR_ARCHITECTURE -eq "ARM64" -and -not (Test-Path $x64DesktopRuntime)) {
    winget install --id Microsoft.DotNet.DesktopRuntime.8 -e --architecture x64 --force --accept-package-agreements --accept-source-agreements
}

Write-Host "Twingate may require manual interaction:" -ForegroundColor Yellow
Write-Host "  1. Follow the installation wizard all the way through." -ForegroundColor Yellow
Write-Host "  2. When the OS prompts, allow the VPN profile/configuration and the network extension." -ForegroundColor Yellow

winget install --id Twingate.Client -e --accept-package-agreements --accept-source-agreements
