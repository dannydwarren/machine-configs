$OneDrive = "$env:UserProfile\OneDrive"
mkdir $OneDrive -ErrorAction Ignore

#TODO: renamed from $git
$src = "C:\src"
mkdir $src -ErrorAction Ignore

#TODO: renamed from $backupDir
$backupNoSync = "C:\backup-no-sync"
mkdir $backupNoSync -ErrorAction Ignore

$tmp = "C:\tmp"
mkdir $tmp -ErrorAction Ignore

#TODO: need to understand this better
$tmpToDelete = "$tmp\ToDelete\$(Get-Date -Format "yyyyMM")"

$transcripts = "$backupNoSync\PowerShell Transcripts"
mkdir $transcripts -ErrorAction Ignore

$InformationPreference = 'Continue'

function setLocationToSrc() {
    if (($Pwd -like '*system32*') -or ($Pwd -like '*danny*')) {
        Set-Location $src
    }
}
setLocationToSrc

function applyTheme() {
    $env:POSH_GIT_ENABLED = $true
    oh-my-posh init pwsh --config c:\src\machine-configs\powershell\ben.omp.json | Invoke-Expression
    Enable-PoshLineError
}

if ($PSVersionTable.PSVersion.Major -gt 5) {
    applyTheme
}

Set-Alias -Name tf -Value terraform.exe
Set-Alias -Name android -Value scrcpy

# TODO: Run this as nightly job
function clear-clipboard {
    Restart-Service -Name "cbdhsvc*" -force
}

function clone-repo($repositoryOrganization, $repositoryName){
    if(![System.IO.File]::Exists("$src\$repositoryName")){
        git clone "https://github.com/$repositoryOrganization/$repositoryName.git"
    }
}
