$currentScriptDirectory = Split-Path -Path $PSCommandPath

$civ6SavesNonOneDriveDirectory = "C:\Users\danny\Documents\My Games\Sid Meier's Civilization VI\Saves"
$civ6SavesBackupDirectory = "$currentScriptDirectory/Civ6Saves"

if (Test-Path -Path $civ6SavesNonOneDriveDirectory){
    Write-Host "Backing up CIV 6 saves not in OneDrive to $civ6SavesBackupDirectory"
    # 7z a -t7z "civ6saves.7z" $civ6SavesNonOneDrive #produces a file too large for GitHub
    # Consider zipping each save file
    # Consider copying to OneDrive (poor mans sync)
    Copy-Item -Path $civ6SavesNonOneDriveDirectory\* -Destination $civ6SavesBackupDirectory -Recurse -Force
}