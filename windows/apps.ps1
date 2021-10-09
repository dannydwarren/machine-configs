function UninstallBlock([string]$AppName) {
    Block "Uninstall Appx package $AppName" {
        pwsh -Command "Import-Module Appx -UseWindowsPowerShell; Get-AppxPackage $AppName | Remove-AppxPackage"
    } {
        !(Get-AppxPackage $AppName)
    }
}

FirstRunBlock "Clean up items on desktop" {
    DeleteDesktopShortcut "Microsoft Edge"
}

FirstRunBlock "Configure OneNote" {
    Write-ManualStep "Start OneNote notebooks syncing"
    start "shell:AppsFolder\$(Get-StartApps "OneNote for Windows 10" | select -ExpandProperty AppId)"
    WaitWhileProcess onenoteim
}

UninstallBlock Microsoft.BingWeather
UninstallBlock Microsoft.GetHelp
UninstallBlock Microsoft.Getstarted # Tips
UninstallBlock Microsoft.OneConnect # Mobile Plans
UninstallBlock Microsoft.MicrosoftSolitaireCollection
UninstallBlock Microsoft.WindowsFeedbackHub
UninstallBlock Microsoft.MicrosoftOfficeHub
UninstallBlock Microsoft.WindowsMaps
UninstallBlock SpotifyAB.SpotifyMusic
