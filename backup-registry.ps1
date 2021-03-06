$now = (Get-Date -Format o) -replace ":", "_"

function BackupRegistryRootKey($rootkey, $backupNoSync) {
    Write-Output "Backing up $rootkey"
    reg export $rootkey "$backupNoSync\reg-$now-$rootkey.reg"
}

("HKCR", "HKCU", "HKLM", "HKU", "HKCC") | % { BackupRegistryRootKey $_ }
