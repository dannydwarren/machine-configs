param([switch]$DryRun, [switch]$SkipBackup, [string]$Run)

# TODO: Why did I put this here?
#scoop hold git-with-openssh

. $PSScriptRoot\config-functions.ps1

# TODO: store in profile
$backupNoSync = 'C:\ConfigurationBackup'
mkdir $backupNoSync -ErrorAction Ignore

Block "Configure for" {
    $configureForOptions = {
        $forHome = "home"
        $forWork = "work"
        $forTest = "test"
    }
    . $configureForOptions

    while (($configureFor = (Read-Host "Configure for ($forHome,$forWork,$forTest)")) -notin @($forHome, $forWork, $forTest)) { }

    if (!(Test-Path $profile)) {
        New-Item $profile -Force
    }

    Add-Content -Path $profile -Value "`n"
    Add-Content -Path $profile $configureForOptions
    Add-Content -Path $profile -Value "`$configureFor = `"$configureFor`""
    Add-Content -Path $profile {
        function Configured([Parameter(Mandatory = $true)][ValidateSet("home", "work", "test")][string]$for) {
            if (!$configureFor) {
                throw '$configureFor not set'
            }
            $for -eq $configureFor
        }
    }
} {
    (Test-Path $profile) -and (Select-String "\`$configureFor" $profile) # -Pattern is regex
}
if (!$DryRun -and !$Run) { . $profile } # make profile available to scripts below

Block "Backup Registry" {
    if (!(Configured $forTest)) {
        & $PSScriptRoot\backup-registry.ps1
    }
} {
    $SkipBackup
}

& $PSScriptRoot\powershell\config.ps1
if (!$DryRun -and !$Run) { . $profile } # make profile available to scripts below


Block "Rename computer" {
    Write-ManualStep
    # TODO: append YYMMDD
    Rename-Computer -NewName (Read-Host "Set computer name to")
} {
    $env:ComputerName -notlike 'desktop-*' -and $env:ComputerName -notlike 'laptop-*'
} -RequiresReboot

Block "System > Multitasking > Alt + Tab > Pressing Alt + Tab shows = Open windows only" {
    Set-RegistryValue "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" MultiTaskingAltTabFilter 3
} -RequiresReboot

# NOTE: Waits for printer dialog to close - Ben said this has not been working well lately...
if (!(Configured $forHome) -and !(Configured $forTest)) {
    FirstRunBlock "Devices > Printers & scanners > Add a printer or scanner > The printer that I want isn't listed" {
        Write-ManualStep "Select a shared printer by name = \\{Server}\{Printer}"
        rundll32 printui.dll PrintUIEntry /im
        WaitWhileProcess rundll32
    }
}

& $PSScriptRoot\windows\config.ps1
& $PSScriptRoot\install\install.ps1