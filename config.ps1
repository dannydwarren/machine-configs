param([switch]$DryRun, [switch]$SkipBackup, [string]$Run)

Write-Host 'Starting to configure this machine'

. $PSScriptRoot\config-functions.ps1