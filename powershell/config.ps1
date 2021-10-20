Block "PowerShell Transcripts" {
    mkdir "$backupDir\PowerShell Transcripts" -ErrorAction Ignore
}

Block "Configure profile.ps1" {
    Add-Content -Path $profile -Value "`n. $PSScriptRoot\profile.ps1"
} {
    (Test-Path $profile) -and (Select-String "$($PSScriptRoot -replace "\\", "\\")\\profile.ps1" $profile) # <original> is regex, <substitute> is PS string
}

if (!(Configured $forTest)) {
    FirstRunBlock "Update PS help" {
        Update-Help -ErrorAction Ignore
    }
}
