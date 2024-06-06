Import-Module PackageManagement -UseWindowsPowerShell
Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force
Import-Module PowerShellGet -UseWindowsPowerShell
Register-PSRepository -Default