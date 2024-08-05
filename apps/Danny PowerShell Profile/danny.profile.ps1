$OneDrive = "$env:UserProfile\OneDrive"
mkdir $OneDrive -ErrorAction Ignore

#TODO: renamed from $git
$src = "C:\src"
mkdir $src -ErrorAction Ignore

#TODO: renamed from $backupDir
$backupNoSync = "C:\backup-no-sync"
mkdir $backupNoSync -ErrorAction Ignore

$tmp = "C:\tmp"
mkdir $tmp -ErrorAction Ignore

#TODO: need to understand this better
$tmpToDelete = "$tmp\ToDelete\$(Get-Date -Format "yyyyMM")"

$transcripts = "$backupNoSync\PowerShell Transcripts"
mkdir $transcripts -ErrorAction Ignore

$InformationPreference = 'Continue'

function setLocationToSrc() {
  if (($Pwd -like '*system32*') -or ($Pwd -like '*danny*')) {
    Set-Location $src
  }
}
setLocationToSrc

function applyTheme() {
  $env:POSH_GIT_ENABLED = $true
  oh-my-posh init pwsh --config c:\src\machine-configs\powershell\ben.omp.json | Invoke-Expression
  Enable-PoshLineError
}

if ($PSVersionTable.PSVersion.Major -gt 5) {
  applyTheme
}

Set-Alias -Name tf -Value terraform.exe
Set-Alias -Name android -Value scrcpy

# TODO: Run this as nightly job
function clear-clipboard {
  Restart-Service -Name "cbdhsvc*" -force
}

function clone-repo($repositoryOrganization, $repositoryName) {
  if (![System.IO.File]::Exists("$src\$repositoryName")) {
    git clone "https://github.com/$repositoryOrganization/$repositoryName.git"
  }
}

#OPTIONS: dev-phoenix
function setAwsProfile($awsProfile) {
  aws s3 ls --profile $awsProfile
  setJnLocalEnv $awsProfile
}
function loginAws($awsProfile) {
  aws sso login --profile $awsProfile
  setJnLocalEnv $awsProfile
}
function setJnLocalEnv($awsProfile) {
  $env:AWS_PROFILE = $awsProfile
  Write-Host "Profile set to: $env:AWS_PROFILE"
  $env:AWS_REGION = (aws configure get region --profile $env:AWS_PROFILE)
  Write-Host "Region set to: $env:AWS_REGION"
}
function generateLocalEnv() {
  Write-Host "Profile set to: $env:AWS_PROFILE"
  Write-Host "Region set to: $env:AWS_REGION"
  
  #to find something aws ecs list-task-definitions | select-string "the thing"
  runJnLocal webapp-api2-dev "$src\webappnew\future\api\.env"
  runJnLocal jncore-nodeapi-dev "$src\webappnew\NodeServer\.env"
  runJnLocal webapp-worker-dev "$src\webappnew\Future\Worker\.env"
}
function runJnLocal($taskDef, $outputFilePath) {
  $overridePath = "$HOME\JobNimbus\local-overrides.json"
  jnlocal dotenv --source taskdef $taskDef -o $outputFilePath -w $overridePath
  Write-Host "Secrets stored for $taskDef in $outputFilePath"
}

