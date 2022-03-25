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

# Debugging Notes

## xunit Console Logging
One of the things that makes testing difficult is that xunit does automatically write `Console.WriteLine()` statements to the test output. It also does not write to the debug console when debugging tests. I have found that when using JetBrains Rider in my setup on Windows it writes the test output to this directory `~\AppData\Local\JetBrains\Rider2021.3\log\UnitTestLogs\Sessions`. Obviously that will change based on the version of Rider.

One of my goals is to build a way for xunit to output to a `List<string>` so it can be analyzed at test assertion time. For a CLI tool this seems like an important thing to be able to test. If you know how to do this please feel free to reach out or submit a PR. 😁 Cheers!