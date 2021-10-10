function InstallFollowup([string]$ProgramName, [scriptblock]$Followup) {
    ConfigFollowup "Finish $ProgramName Install" $Followup
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

InstallFromGitHubBlock benallred YouTubeToPlex

