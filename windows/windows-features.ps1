function WindowsFeatureBlock([string]$Comment, [string]$FeatureName) {
    Block "Windows Features > $Comment = On" {
        # https://github.com/PowerShell/PowerShell/issues/13866
        powershell -Command "Enable-WindowsOptionalFeature -Online -FeatureName $FeatureName -All -NoRestart"
    } {
        ((powershell -Command "(Get-WindowsOptionalFeature -Online -FeatureName $FeatureName).State -eq 'Enabled'") -eq "True")
    } -RequiresReboot
}

WindowsFeatureBlock ".NET Framework 3.5 (includes .NET 2.0 and 3.0)" NetFx3

WindowsFeatureBlock "Internet Information Services > Web Management Tools > IIS Management Console" IIS-ManagementConsole
WindowsFeatureBlock "Internet Information Services > World Wide Web Services > Application Development Features > ASP.NET 4.x" IIS-ASPNET45
# (What needs Hyper-V these days?) - Maybe try having it disabled and use VMWare/VirtualBox?
# Need to figure out if Docker with WSL 2 is OK w/o Hyper-V
WindowsFeatureBlock "Hyper-V" Microsoft-Hyper-V
WindowsFeatureBlock "Windows Subsystem for Linux" Microsoft-Windows-Subsystem-Linux
WindowsFeatureBlock "Virtual Machine Platform" VirtualMachinePlatform # part of updating to WSL 2

Block "Update to WSL 2" {
    ConfigFollowup "Update to WSL 2" {
        #https://docs.microsoft.com/en-us/windows/wsl/wsl2-kernel
        #https://docs.microsoft.com/en-us/windows/wsl/install-win10#:~:text=install%20the%20MSI%20from%20that%20page%20on%20our%20documentation%20to%20install%20a%20Linux%20kernel%20on%20your%20machine%20for%20WSL%202%20to%20use
        Download-File https://wslstorestorage.blob.core.windows.net/wslblob/wsl_update_x64.msi $env:tmp\wsl_update_x64.msi
        . $env:tmp\wsl_update_x64.msi /passive /norestart
        wsl --set-default-version 2
    }
} {
    (Get-Command wsl -ErrorAction Ignore) -and ((wsl -l) -replace "`0", "" | Select-String "Windows Subsystem for Linux Distributions:")
} -RequiresReboot
