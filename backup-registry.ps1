$now = (Get-Date -Format o) -replace ":", "_"

function BackupRegistryRootKey($rootkey, $backupDir) {
    Write-Output "Backing up $rootkey"
    reg export $rootkey "$backupDir\reg-$now-$rootkey.reg"
}

("HKCR", "HKCU", "HKLM", "HKU", "HKCC") | % { BackupRegistryRootKey $_ }
