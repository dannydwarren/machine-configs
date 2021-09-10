function WaitWhile([scriptblock]$ScriptBlock, [string]$WaitingFor) {
    while (Invoke-Command $ScriptBlock) {
        Write-Host -ForegroundColor Yellow $WaitingFor
        sleep -s 10
    }
}

# TODO: Why do we need to launch it first?
. (. "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -property productPath) $PSCommandPath
WaitWhile { !(Get-ChildItem "HKCU:\Software\Microsoft\VisualStudio" | ? { $_.PSChildName -match "^\d\d.\d_" }) } "Waiting for Visual Studio registry key"

$visualStudioVersionKey = Get-ChildItem "HKCU:\Software\Microsoft\VisualStudio" | ? { $_.PSChildName -match "^\d\d.\d_" } | Select-Object -Last 1
# TODO: What is this for again?
Set-ItemProperty Registry::$visualStudioVersionKey -Name UseSolutionNavigatorGraphProvider -Value 0
            