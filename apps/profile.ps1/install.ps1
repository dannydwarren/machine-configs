New-Item -Path $profile -ItemType File -ErrorAction Ignore
Add-Content -Path $profile -Value "`n. $($PSScriptRoot)\profile.ps1"