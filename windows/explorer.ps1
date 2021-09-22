Block "Home > Organize > Delete (options) > Show recycle confirmation = On" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" -Name ConfirmFileDelete -Value 1
}
Block "View > Options > General > Open File Explorer to = This PC" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name LaunchTo -Value 1
}
Block "View > Options > View > Advanced settings > Files and Folders > Hidden files and folders = Show hidden files, folders, and drives" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name Hidden -Value 1
}
Block "View > Options > View > Advanced settings > Files and Folders > Hide empty drives = Off" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name HideDrivesWithNoMedia -Value 0
}
Block "View > Options > View > Advanced settings > Files and Folders > Hide extensions for known file types = Off" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name HideFileExt -Value 0
}
Block "View > Options > View > Advanced settings > Files and Folders > Hide folder merge conflicts = Off" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name HideMergeConflicts -Value 0
}
Block "View > Options > View > Advanced settings > Files and Folders > Hide protected operating system files (Recommended) = Off" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name ShowSuperHidden -Value 1
} -RequiresReboot
Block "View > Options > View > Advanced settings > Files and Folders > Restore previous folder windows at logon = On" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name PersistBrowsers -Value 1
}
Block "View > Options > View > Advanced settings > Files and Folders > Show sync provider notifications = Off" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name ShowSyncProviderNotifications -Value 0
}
Block "View > Options > View > Advanced settings > Navigation pane > Expand to open folder = On" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name NavPaneExpandToCurrentFolder -Value 1
}
Block "View > Options > View > Advanced settings > Navigation pane > Show all folders = On" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" -Name NavPaneShowAllFolders -Value 1
}
Block "Hide Libraries in Navigation pane" {
    Set-RegistryValue "HKCU:\Software\Classes\CLSID\{031E4825-7B94-4dc3-B131-E946B44C8DD5}\ShellFolder" -Name Attributes -Value 0xb090010d
} -RequiresReboot
Block "Hide Control Panel in Navigation pane" {
    Set-RegistryValue "HKCU:\Software\Classes\CLSID\{26EE0668-A00A-44D7-9371-BEB064C98683}\ShellFolder" -Name Attributes -Value 0xa0900004
} -RequiresReboot
Block "Internet Options > Advanced > Browsing > Use inline AutoComplete in File Explorer and Run Dialog = On" {
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\AutoComplete" -Name "Append Completion" -Value "yes"
}
Block "Hide Folders Section in This PC" {
    & "$PSScriptRoot\Hide Folders Section in This PC.ps1"
}
