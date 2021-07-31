function InstallFollowup([string]$ProgramName, [scriptblock]$Followup) {
    ConfigFollowup "Finish $ProgramName Install" $Followup
}


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

InstallFromGitHubBlock benallred YouTubeToPlex

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
