# machine-configs

```powershell
$bootstrapDuration = [Diagnostics.Stopwatch]::StartNew()
Set-ExecutionPolicy RemoteSigned -Force
Invoke-Command {
    $asset = (iwr -useb https://api.github.com/repos/dannydwarren/machine-configs/releases/latest | ConvertFrom-Json).assets | ? { $_.name -like "*.exe" }
    $downloadUrl = $asset | select -exp browser_download_url
    iwr ($downloadUrl) -OutFile "$HOME\Downloads\Configurator.exe"
}
$downloadDuration = $bootstrapDuration.Elapsed

$bootstrapDuration.Restart()
."$HOME\Downloads\Configurator.exe" --manifest-path "https://raw.githubusercontent.com/dannydwarren/machine-configs/main/manifests/danny.manifest.json" --environments "Personal"
Write-Output "Download duration: $($downloadDuration)"
Write-Output "Configurator duration: $($bootstrapDuration.Elapsed)"
```


# Non-NuGet Dependencies

## Emmersion.Http
Source: https://github.com/emmersion/Emmersion.Http

Justification: The package put out by Emmersion as of July 20, 2021 targets `netcoreapp3.1`. I want to consume a `netstandard2.1` version for flexibility.
