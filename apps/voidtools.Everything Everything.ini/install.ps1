Copy-Item -Path "$($PSScriptRoot)/Everything.ini" -Destination "$env:ProgramFiles\Everything\Everything.ini"

$oldpath = (Get-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH).path
$newpath = "$oldpath;$env:ProgramFiles\Everything"
Set-ItemProperty -Path 'Registry::HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Session Manager\Environment' -Name PATH -Value $newpath

$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")

# Seems like this call sometimes freezes up. Will move this app to be last for now so a freeze up won't affect other apps.
everything -install-run-on-system-startup;everything -startup