$bashProfile = "$HOME\.bash_profile"
(Test-Path $bashProfile) -and (Select-String "$($PSScriptRoot)\danny.profile.sh" $bashProfile -SimpleMatch)
