# machine-configs

```powershell
$bootstrapStopwatch = [Diagnostics.Stopwatch]::StartNew()
Set-ExecutionPolicy RemoteSigned -Force

Write-Output "Initializing profile at: $profile"
mkdir ([System.IO.Path]::GetDirectoryName($profile)) -ErrorAction Ignore
New-Item -Path $profile -ItemType "file" -ErrorAction Ignore

Invoke-Command {
    $asset = (iwr -useb https://api.github.com/repos/dannydwarren/machine-configs/releases/latest | ConvertFrom-Json).assets | ? { $_.name -like "*.exe" }
    $downloadUrl = $asset | select -exp browser_download_url
    Start-BitsTransfer -Source $downloadUrl -Destination "$HOME\Downloads\Configurator.exe"
}
$downloadDuration = $bootstrapStopwatch.Elapsed
Write-Output "Download duration: $($downloadDuration)"

$bootstrapStopwatch.Restart()
."$HOME\Downloads\Configurator.exe" --manifest-path "https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/danny.manifest.json" --environments "Personal"
Write-Output "Download duration: $($downloadDuration)"
$bootstrapDuration = $bootstrapStopwatch.Elapsed
Write-Output "Configurator duration: $($bootstrapDuration)"
$totalDuration = $downloadDuration + $bootstrapDuration
Write-Output "Total duration: $($totalDuration)"
$bootstrapStopwatch.Stop()
```


# Non-NuGet Dependencies

## Emmersion.Http
Source: https://github.com/emmersion/Emmersion.Http

Justification: The package put out by Emmersion as of July 20, 2021 targets `netcoreapp3.1`. I want to consume a `netstandard2.1` version for flexibility.
