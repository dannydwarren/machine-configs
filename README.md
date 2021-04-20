# machine-configs

```powershell
Set-ExecutionPolicy RemoteSigned -Force
iwr -useb get.scoop.sh | iex
scoop install git-with-openssh
git clone https://github.com/dannydwarren/machine-configs.git C:\src\machine-configs
Invoke-Expression "& { $(Invoke-RestMethod 'https://aka.ms/install-powershell.ps1') } -UseMSI -Quiet"
start pwsh "-NoExit", "-WindowStyle", "Maximized", "-File", "C:\src\machine-configs\config.ps1"
```