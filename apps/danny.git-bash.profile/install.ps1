$bashProfile = "$HOME\.bash_profile"
New-Item -Path $bashProfile -ItemType File -ErrorAction Ignore
Add-Content -Path $bashProfile -Value "`n. '$($PSScriptRoot)\danny.profile.sh'"
