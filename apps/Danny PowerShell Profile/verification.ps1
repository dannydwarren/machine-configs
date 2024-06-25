(Test-Path $profile) -and (Select-String "$($PSScriptRoot)\danny.profile.ps1" $profile -SimpleMatch)
