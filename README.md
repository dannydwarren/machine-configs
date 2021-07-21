# machine-configs

```powershell
Set-ExecutionPolicy RemoteSigned -Force
iwr -useb get.scoop.sh | iex
scoop install git-with-openssh
git clone https://github.com/dannydwarren/machine-configs.git C:\src\machine-configs
Invoke-Expression "& { $(Invoke-RestMethod 'https://aka.ms/install-powershell.ps1') } -UseMSI -Quiet"
start pwsh "-NoExit", "-WindowStyle", "Maximized", "-File", "C:\src\machine-configs\config.ps1"
```

# Non-NuGet Dependencies

## Emmersion.Http
Source: https://github.com/emmersion/Emmersion.Http

Justification: The package put out by Emmersion as of July 20, 2021 targets `netcoreapp3.1`. I want to consume a `netstandard2.1` version for flexibility.
