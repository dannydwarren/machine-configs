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
    if (($PSScriptRoot -like '*system32*') -or ($PSScriptRoot -like '*danny*')) {
        Set-Location $src
    }
}
setLocationToSrc

try { $null = gcm pshazz -ea stop; pshazz init 'default' } catch { }

Set-Alias -Name tf -Value terraform.exe
Set-Alias -Name android -Value scrcpy

function clear-clipboard {
    Restart-Service -Name "cbdhsvc*" -force
}