$openssl = "C:\Program Files\Git\usr\bin\openssl.exe"
$snowflakeDir = "$env:USERPROFILE\.snowflake"

if (-not (Test-Path $snowflakeDir)) {
    New-Item -ItemType Directory -Path $snowflakeDir -Force | Out-Null
}

$privateKey = Join-Path $snowflakeDir "rsa_key.p8"
$publicKey = Join-Path $snowflakeDir "rsa_key.pub"

if (Test-Path $privateKey) {
    Write-Host "Snowflake key pair already exists at $snowflakeDir"
} else {
    & $openssl genrsa 2048 | & $openssl pkcs8 -topk8 -nocrypt -inform PEM -out $privateKey
    & $openssl rsa -in $privateKey -pubout -out $publicKey
    Write-Host "Snowflake key pair generated at $snowflakeDir"
}

explorer.exe $snowflakeDir
