function InstallFollowup([string]$ProgramName, [scriptblock]$Followup) {
    ConfigFollowup "Finish $ProgramName Install" $Followup
}

Block "Configure scoop extras bucket" {
    scoop bucket add extras
} {
    scoop bucket list | Select-String extras
}

Block "Configure scoop nonportable bucket" {
    scoop bucket add nonportable
} {
    scoop bucket list | Select-String nonportable
}

if (!(Configured $forKids)) {
    Block "Install Edge (Dev)" {
        Download-File "https://go.microsoft.com/fwlink/?linkid=2069324&Channel=Dev&language=en&Consent=1" $env:tmp\MicrosoftEdgeSetupDev.exe
        . $env:tmp\MicrosoftEdgeSetupDev.exe
        DeleteDesktopShortcut "Microsoft Edge Dev"
    } {
        Test-ProgramInstalled "Microsoft Edge Dev"
    }
}

# TODO: Ask Ben - Do you feel like you've enjoyed this? Is it worth it?
Block "Install Authy" {
    Download-File "https://electron.authy.com/download?channel=stable&arch=x64&platform=win32&version=latest&product=authy" "$env:tmp\Authy Desktop Setup.exe"
    . "$env:tmp\Authy Desktop Setup.exe"
    DeleteDesktopShortcut "Authy Desktop"
} {
    Test-ProgramInstalled "Authy Desktop"
}

InstallFromScoopBlock Everything everything {
    Copy-Item $PSScriptRoot\..\programs\Everything.ini (scoop prefix everything)
    everything -install-run-on-system-startup
    everything -startup
}

InstallFromScoopBlock .NET dotnet-sdk

InstallFromScoopBlock nvm nvm {
    nvm install latest
    nvm use (nvm list)
}

# TODO: Ask Ben - Is this required for any of this config system
# InstallFromScoopBlock Yarn yarn

# Move to work folder for PS?
# InstallFromScoopBlock "AWS CLI" aws

# TODO: Ask Ben
Block "Install VS Code" {
    Download-File https://aka.ms/win32-x64-user-stable $env:tmp\VSCodeUserSetup-x64.exe
    . $env:tmp\VSCodeUserSetup-x64.exe /SILENT /TASKS="associatewithfiles,addtopath" /LOG=$env:tmp\VSCodeInstallLog.txt
    WaitWhile { !(Test-ProgramInstalled "Visual Studio Code") } "Waiting for VS Code to be installed"
    $codeCmd = "$env:LocalAppData\Programs\Microsoft VS Code\bin\code.cmd"
    Write-ManualStep "Turn on Settings Sync"
    Write-ManualStep "`tReplace Local"
    Write-ManualStep "Watch log with ctrl+shift+u"
    Write-ManualStep "Show synced data"
    Write-ManualStep "`tUpdate name of synced machine"
    . $codeCmd
} {
    Test-ProgramInstalled "Microsoft Visual Studio Code (User)"
}

Block "Install Visual Studio" {
    # https://visualstudio.microsoft.com/downloads/
    $downloadUrl = (iwr "https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Professional&rel=16" -useb | sls "https://download\.visualstudio\.microsoft\.com/download/pr/.+?/vs_Professional.exe").Matches.Value
    Download-File $downloadUrl $env:tmp\vs_professional.exe
    # https://docs.microsoft.com/en-us/visualstudio/install/workload-and-component-ids?view=vs-2019
    # Microsoft.VisualStudio.Workload.ManagedDesktop    .NET desktop development
    # Microsoft.VisualStudio.Workload.NetWeb            ASP.NET and web development
    # Microsoft.VisualStudio.Workload.NetCoreTools      .NET Core cross-platform development
    # https://docs.microsoft.com/en-us/visualstudio/install/command-line-parameter-examples?view=vs-2019#using---wait
    $vsInstallArgs = '--passive', '--norestart', '--wait', '--includeRecommended', '--add', 'Microsoft.VisualStudio.Workload.ManagedDesktop', '--add', 'Microsoft.VisualStudio.Workload.NetWeb', '--add', 'Microsoft.VisualStudio.Workload.NetCoreTools'
    Start-Process $env:tmp\vs_professional.exe $vsInstallArgs -Wait
    InstallFollowup "Visual Studio" {
        . (. "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -property productPath) $PSCommandPath
        WaitWhile { !(Get-ChildItem "HKCU:\Software\Microsoft\VisualStudio" | ? { $_.PSChildName -match "^\d\d.\d_" }) } "Waiting for Visual Studio registry key"
        & "$git\configs\programs\Visual Studio - Hide dynamic nodes in Solution Explorer.ps1"
    }

    function InstallVisualStudioExtension([string]$Publisher, [string]$Extension) {
        $downloadUrl = (iwr "https://marketplace.visualstudio.com/items?itemName=$Publisher.$Extension" -useb | sls "/_apis/public/gallery/publishers/$Publisher/vsextensions/$Extension/(\d+\.?)+/vspackage").Matches.Value | % { "https://marketplace.visualstudio.com$_" }
        Download-File $downloadUrl $env:tmp\$Publisher.$Extension.vsix
        $vsixInstaller = . "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -property productPath | Split-Path | % { "$_\VSIXInstaller.exe" }
        $installArgs = "/quiet", "/admin", "$env:tmp\$Publisher.$Extension.vsix"
        Write-Output "Installing $Extension"
        Start-Process $vsixInstaller $installArgs -Wait
    }

    InstallVisualStudioExtension VisualStudioPlatformTeam SolutionErrorVisualizer
    InstallVisualStudioExtension VisualStudioPlatformTeam FixMixedTabs
    InstallVisualStudioExtension VisualStudioPlatformTeam PowerCommandsforVisualStudio
    # InstallVisualStudioExtension maksim-vorobiev PeasyMotion
    InstallVisualStudioExtension Shanewho IHateRegions
} {
    Test-ProgramInstalled "Visual Studio Professional 2019"
}

# Block "Install ReSharper" {
#     $resharperJson = (iwr "https://data.services.jetbrains.com/products/releases?code=RSU&latest=true&type=release" -useb | ConvertFrom-Json)
#     $downloadUrl = $resharperJson.RSU[0].downloads.windows.link
#     $fileName = Split-Path $downloadUrl -Leaf
#     Download-File $downloadUrl $env:tmp\$fileName
#     . $env:tmp\$fileName /SpecificProductNames=ReSharper /VsVersion=16.0 /Silent=True
#     # Activation:
#     #   ReSharper command line activation not currently available:
#     #   https://resharper-support.jetbrains.com/hc/en-us/articles/206545049-Can-I-enter-License-Key-License-Server-URL-via-Command-Line-when-installing-ReSharper-
#     # Settings:
#     #   No CLI that I can find to import settings file
#     #   It might be roamed?
#     #   Or try editing $env:AppData\JetBrains\Shared\vAny\GlobalSettingsStorage.DotSettings
#     #       <s:Boolean x:Key="/Default/Environment/InjectedLayers/FileInjectedLayer/=8232C3A8D8B5804BBE2C12625C76862A/@KeyIndexDefined">True</s:Boolean>
#     #       <s:String x:Key="/Default/Environment/InjectedLayers/FileInjectedLayer/=8232C3A8D8B5804BBE2C12625C76862A/AbsolutePath/@EntryValue">C:\BenLocal\git\configs\programs\resharper.DotSettings</s:String>
#     #       <s:Boolean x:Key="/Default/Environment/InjectedLayers/InjectedLayerCustomization/=File8232C3A8D8B5804BBE2C12625C76862A/@KeyIndexDefined">True</s:Boolean>
#     #       <s:Double x:Key="/Default/Environment/InjectedLayers/InjectedLayerCustomization/=File8232C3A8D8B5804BBE2C12625C76862A/RelativePriority/@EntryValue">1</s:Double>
#     # Conflicting shortcuts
#     #   Can't find a setting to disable the popup
#     #   Perhaps edit $env:LocalAppData\JetBrains\ReSharper\vAny\vs16.0_ef96ec49\vsActionManager.DotSettings
#     #       Remove all keys with "ConflictingActions" and corresponding "ActionsWithShortcuts"?
#     # External source navigation
#     #   Perhaps edit $env:AppData\JetBrains\Shared\vAny\GlobalSettingsStorage.DotSettings
#     #       Remove? <s:String x:Key="/Default/Housekeeping/OptionsDialog/SelectedPageId/@EntryValue">ExternalSources</s:String>
#     #       Edit? <s:Int64 x:Key="/Default/Environment/SearchAndNavigation/DefaultOccurrencesGroupingIndices/=JetBrains_002EReSharper_002EFeature_002EServices_002ENavigation_002EDescriptors_002ESearchUsagesDescriptor/@EntryIndexedValue">12</s:Int64>
#     #             <s:Int64 x:Key="/Default/Environment/SearchAndNavigation/DefaultOccurrencesGroupingIndices/=JetBrains_002EReSharper_002EFeature_002EServices_002ENavigation_002EDescriptors_002ESearchUsagesDescriptor/@EntryIndexedValue">12</s:Int64>
#     #       Neither of the above seem to change when selecting an option in the popup
# } {
#     Test-ProgramInstalled "JetBrains ReSharper in Visual Studio Professional 2019"
# }

if (!(Configured $forTest)) {
    Block "Install Docker" {
        Download-File https://desktop.docker.com/win/stable/Docker%20Desktop%20Installer.exe "$env:tmp\Docker Desktop Installer.exe"
        # https://github.com/docker/for-win/issues/1322
        . "$env:tmp\Docker Desktop Installer.exe" install --quiet | Out-Default
        DeleteDesktopShortcut "Docker Desktop"
        ConfigureNotifications "Docker Desktop"
        WaitWhile { !(Get-ItemProperty "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name "Docker Desktop" -ErrorAction Ignore) } "Waiting for Docker startup registry key"
        Remove-ItemProperty "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name "Docker Desktop"
    } {
        Test-ProgramInstalled "Docker Desktop"
    } -RequiresReboot
}

# Block "Install AutoHotkey" {
#     Download-File https://www.autohotkey.com/download/ahk-install.exe $env:tmp\ahk-install.exe
#     . $env:tmp\ahk-install.exe /S /IsHostApp
# } {
#     Test-ProgramInstalled AutoHotkey
# }

Block "Install Slack" {
    Download-File https://downloads.slack-edge.com/releases_x64/SlackSetup.exe $env:tmp\SlackSetup.exe
    . $env:tmp\SlackSetup.exe
    if (!(Configured $forWork)) {
        WaitWhile { !(Get-ItemProperty "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name com.squirrel.slack.slack -ErrorAction Ignore) } "Waiting for Slack startup registry key"
        Remove-ItemProperty "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name com.squirrel.slack.slack
    }
    DeleteDesktopShortcut Slack
    ConfigureNotifications Slack
} {
    Test-ProgramInstalled Slack
}

Block "Install Office" {
    # https://www.microsoft.com/en-in/download/details.aspx?id=49117
    $downloadUrl = (iwr "https://www.microsoft.com/en-in/download/confirmation.aspx?id=49117" -useb | sls "https://download\.microsoft\.com/download/.+?/officedeploymenttool_.+?.exe").Matches.Value
    Download-File $downloadUrl $env:tmp\officedeploymenttool.exe
    . $env:tmp\officedeploymenttool.exe /extract:$env:tmp\officedeploymenttool /passive /quiet
    WaitWhile { !(Test-Path $env:tmp\officedeploymenttool\setup.exe) } "Waiting for Office setup to be extracted"
    . $env:tmp\officedeploymenttool\setup.exe /configure $PSScriptRoot\OfficeConfiguration.xml
    # TODO: Activate
    #   Observed differences
    #       Manual install and activation
    #           Word > Account: Product Activated \ Microsoft Office Professional Plus 2019
    #           cscript "C:\Program Files (x86)\Microsoft Office\Office16\OSPP.VBS" /dstatus
    #               LICENSE NAME: Office 19, Office19ProPlus2019MSDNR_Retail edition
    #               LICENSE DESCRIPTION: Office 19, RETAIL channel
    #               LICENSE STATUS:  ---LICENSED---
    #               Last 5 characters of installed product key: <correct>
    #       Automated install (product id = Professional2019Retail), no activation
    #           Word > Account: Activation Required \ Microsoft Office Professional 2019
    #       Automated install (product id = Professional2019Retail), activation by filling in PIDKEY
    #           Word > Account: Subscription Product \ Microsoft Office 365 ProPlus
    #           cscript ... /dstatus
    #               Other stuff about a grace period, even though in-product it says activated
    #               Last 5 characters of installed product key: <different>
    #       Automated install (product id = ProPlus2019Volume), activation by filling in PIDKEY
    #           THIS ATTEMPT WORKED
    #           Word > Account: Product Activated \ Microsoft Office Professional Plus 2019
    #           cscript "C:\Program Files\Microsoft Office\Office16\OSPP.VBS" /dstatus
    #               LICENSE NAME: Office 19, Office19ProPlus2019MSDNR_Retail edition
    #               LICENSE DESCRIPTION: Office 19, RETAIL channel
    #               LICENSE STATUS:  ---LICENSED---
    #               Last 5 characters of installed product key: <correct>
    #   Next attempts:
    #       1. Don't put PIDKEY in xml. Activate from command line.
    #           Example:    cscript "C:\Program Files\Microsoft Office\Office16\OSPP.VBS" /inpkey:XXXXX-XXXXX-XXXXX-XXXXX-XXXXX
    #           Actual:     cscript "C:\Program Files\Microsoft Office\Office16\OSPP.VBS" /inpkey:(SecureRead-Host "Office key")
    #           Maybe also: cscript "C:\Program Files\Microsoft Office\Office16\OSPP.VBS" /act
    #           From: https://support.office.com/en-us/article/Change-your-Office-product-key-d78cf8f7-239e-4649-b726-3a8d2ceb8c81#ID0EABAAA=Command_line
    #           From: https://docs.microsoft.com/en-us/deployoffice/vlactivation/tools-to-manage-volume-activation-of-office#ospp
    #       2. SecureRead-Host to get Office key; write to copy of xml in tmp; use tmp configuration
    #       3. Manual activation
} {
    (Test-ProgramInstalled "Microsoft Office") -or (Test-ProgramInstalled "Microsoft 365")
}

InstallFromScoopBlock Sysinternals sysinternals

InstallFromGitHubBlock benallred YouTubeToPlex

Block "Install mob" {
    InstallFromScoopBlock mob mob
    # TODO: Configure aliases
    # TODO: Configure for work
} {
    Test-ProgramInstalled mob
}

Block "Install Steam" {
    Download-File https://steamcdn-a.akamaihd.net/client/installer/SteamSetup.exe $env:tmp\SteamSetup.exe
    Start-Process $env:tmp\SteamSetup.exe "/S" -Wait
    DeleteDesktopShortcut Steam
    if (Configured $forWork) {
        WaitWhile { !(Get-ItemProperty "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name Steam -ErrorAction Ignore) } "Waiting for Steam startup registry key"
        Remove-ItemProperty "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name Steam
    }
} {
    Test-ProgramInstalled "Steam"
}

Block "Install Battle.net" {
    Download-File https://www.battle.net/download/getInstallerForGame $env:tmp\Battle.net-Setup.exe
    . $env:tmp\Battle.net-Setup.exe
    DeleteDesktopShortcut Battle.net
} {
    Test-ProgramInstalled "Battle.net"
}

Block "Install Discord" {
    Download-File https://discord.com/api/download?platform=win $env:tmp\DiscordSetup.exe
    . $env:tmp\DiscordSetup.exe
    DeleteDesktopShortcut Discord
} {
    Test-ProgramInstalled Discord
}

FirstRunBlock "Configure 7-Zip" {
    Set-RegistryValue "HKCU:\SOFTWARE\7-Zip\FM" -Name ShowDots -Value 1
    Set-RegistryValue "HKCU:\SOFTWARE\7-Zip\FM" -Name ShowRealFileIcons -Value 1
    Set-RegistryValue "HKCU:\SOFTWARE\7-Zip\FM" -Name FullRow -Value 1
    Set-RegistryValue "HKCU:\SOFTWARE\7-Zip\FM" -Name ShowSystemMenu -Value 1
    . "$(scoop prefix 7zip)\7zFM.exe"
    Write-ManualStep "Tools >"
    Write-ManualStep "`tOptions >"
    Write-ManualStep "`t`t7-Zip >"
    Write-ManualStep "`t`t`tContext menu items > [only the following]"
    Write-ManualStep "`t`t`t`tOpen archive"
    Write-ManualStep "`t`t`t`tExtract Here"
    Write-ManualStep "`t`t`t`tExtract to <Folder>"
    Write-ManualStep "`t`t`t`tAdd to <Archive>.zip"
    Write-ManualStep "`t`t`t`tCRC SHA >"
    WaitWhileProcess 7zFM
}

InstallFromScoopBlock "Logitech Gaming Software" logitech-gaming-software-np

InstallFromScoopBlock scrcpy scrcpy

InstallFromScoopBlock Paint.NET paint.net

InstallFromScoopBlock "TreeSize Free" treesize-free

# TODO: Review settings that I want
#   Multi-monitor
#   Default meeting uses personal PIN
#   Auto-update enabled
Block "Install Zoom" {
    Download-File https://zoom.us/client/latest/ZoomInstaller.exe $env:tmp\ZoomInstaller.exe
    . "$env:tmp\ZoomInstaller.exe"
    DeleteDesktopShortcut Zoom

    # Configure during install:
    #   https://support.zoom.us/hc/en-us/articles/201362163-Mass-Installation-and-Configuration-for-Windows#h_b82f0349-4d8f-45dd-898a-1ab98389a4b7
    #   Code
    #       Download-File https://zoom.us/client/latest/ZoomInstallerFull.msi $env:tmp\ZoomInstallerFull.msi
    #       msiexec /package "$env:tmp\ZoomInstallerFull.msi" ZRecommend="AutoHideToolbar=1"
    #   I can't get ZRecommend or ZConfig to work (settings are not changed)
    # Group policy:
    #   https://support.zoom.us/hc/en-us/articles/360039100051-Group-Policy-Options-for-the-Windows-Desktop-Client-and-Zoom-Rooms#h_e5b756c6-5e06-4a22-ad78-f19922a6e94f
    #   This works but the downside is the options are uneditable from the UI
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name AlwaysShowMeetingControls -Value 1
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name EnableRemindMeetingTime -Value 1
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name MuteWhenLockScreen -Value 1
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name TurnOffVideoCameraOnJoin -Value 1
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name AlwaysShowVideoPreviewDialog -Value 0
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name SetUseSystemDefaultMicForVOIP -Value 1
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name SetUseSystemDefaultSpeakerForVOIP -Value 1
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name AutoJoinVoIP -Value 1
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name MuteVoIPWhenJoinMeeting -Value 1
    Set-RegistryValue "HKLM:\SOFTWARE\Policies\Zoom\Zoom Meetings\General" -Name EnterFullScreenWhenViewingSharedScreen -Value 1
} {
    Test-ProgramInstalled Zoom
}

Block "Install SQL Server" {
    # https://docs.microsoft.com/en-us/sql/database-engine/install-windows/install-sql-server-from-the-command-prompt
    # Download-File https://go.microsoft.com/fwlink/?linkid=866662 $env:tmp\SQL2019-SSEI-Dev.exe
    Download-File https://go.microsoft.com/fwlink/?linkid=853016 $env:tmp\SQLServer2017-SSEI-Dev.exe
    $installArgs = "/Action=Install", "/IAcceptSqlServerLicenseTerms", "/InstallPath=`"C:\Program Files\Microsoft SQL Server`"", "/Features=FullText", "/SecurityMode=SQL", "/Verbose"
    Start-Process $env:tmp\SQLServer2017-SSEI-Dev.exe $installArgs -Wait
} {
    Test-ProgramInstalled "SQL Server 2017"
}

Block "Install SQL Server Management Studio" {
    # https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms
    Download-File https://aka.ms/ssmsfullsetup $env:tmp\SSMS-Setup-ENU.exe
    $installArgs = "/Passive", "/NoRestart"
    Start-Process $env:tmp\SSMS-Setup-ENU.exe $installArgs -Wait
} {
    Test-ProgramInstalled "SQL Server Management Studio"
}

# TODO: Missing Apps
#   Corsair iCUE Software
#       Restore Settings
#   CPUID HWMonitor
#   Epic Games Launcher
#   FileBot
#       Register
#   Logitech Gaming Software (already installed)
#       Restore Settings
#   Microsoft Azure CLI
#   Microsoft Azure Storage Emulator (v5.10)
#   NVidia Control Panel ???
#   OBS Studio ???
#   Origin
#   Pandora (MS Store)
#   Postman
#   RGB Fusion
#   SyncToy (May need to come from local 'Restoration Tools')
#   Todoist (Windows Installer)
#   Trident Z Lighting Control (G.Skill)
#       Restore Settings
#   Ubuntu (MS Store)
#   Unity ???
#   Uplay

# FOR WORK
#   JetBrains Toolbox
#       Rider
#   Terraform   