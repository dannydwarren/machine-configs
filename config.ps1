param([switch]$DryRun, [switch]$SkipBackup, [string]$Run)
Write-Host 'Starting to configure this machine'
. $PSScriptRoot\config-functions.ps1
$backupDir = 'C:\ConfigurationBackup'
mkdir $backupDir -ErrorAction Ignore

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

Block "Git config" {
    git config --global --add include.path $PSScriptRoot\git\danny.gitconfig
} {
    (git config --get-all --global include.path) -match "danny\.gitconfig"
}
