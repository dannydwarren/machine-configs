# ms-settings: URI scheme reference
# https://docs.microsoft.com/en-us/windows/uwp/launch-resume/launch-settings-app#ms-settings-uri-scheme-reference

Block "Rename computer" {
    Write-ManualStep
    # TODO: append YYMMDD
    Rename-Computer -NewName (Read-Host "Set computer name to")
} {
    $env:ComputerName -notlike 'desktop-*' -and $env:ComputerName -notlike 'laptop-*'
} -RequiresReboot

FirstRunBlock "Add Microsoft account" {
    Write-ManualStep "Sign in with a Microsoft account instead"
    start ms-settings:yourinfo
    WaitWhileProcess SystemSettings
}


& $PSScriptRoot\system.ps1
& $PSScriptRoot\devices.ps1
& $PSScriptRoot\personalization.ps1
& $PSScriptRoot\apps.ps1
& $PSScriptRoot\time-and-language.ps1
& $PSScriptRoot\ease-of-access.ps1
& $PSScriptRoot\windows-features.ps1


FirstRunBlock "Set sign-in options" {
    Write-ManualStep "Windows Hello"
    start ms-settings:signinoptions
}
