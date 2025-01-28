$currentScriptDirectory = Split-Path -Path $PSCommandPath

$source = "$env:APPDATA\Corsair\CUE5"
$destination = "$currentScriptDirectory\CUE5.backup.7z"

if (Test-Path -Path $source){
    # Stop iCUE service
    7z a -t7z "$destination" $source 
    # Restart iCUE service
}