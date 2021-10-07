& $PSScriptRoot\apps.ps1
& $PSScriptRoot\time-and-language.ps1
& $PSScriptRoot\ease-of-access.ps1
& $PSScriptRoot\windows-features.ps1


FirstRunBlock "Set sign-in options" {
    Write-ManualStep "Windows Hello"
    start ms-settings:signinoptions
}
