Set-PSReadLineOption -PredictionSource History -PredictionViewStyle ListView

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

#First Time Setup: https://jobnimbus.atlassian.net/wiki/spaces/DEV/pages/2054684772/Setup+AWS+CLI
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
  
  # To find something aws ecs list-task-definitions | select-string "the thing"
  # To find something aws lambda list-functions | select-string "FunctionName" | select-string "the thing"
  runJnLocalTaskDef "webapp-api2-dev" "$src\webappnew\future\api\.env"
  runJnLocalTaskDef "jncore-nodeapi-dev" "$src\webappnew\NodeServer\.env"
  runJnLocalTaskDef "webapp-worker-dev" "$src\webappnew\Future\Worker\.env"
  runJnLocalTaskDef "custom-fields-api-dev" "$src\custom-fields-backend\api\.env"

  runJnLocalLambdaName "custom-fields-jobs-custom-fields-sync-dev" "$src\custom-fields-backend\lambdas\.env" 
}
function runJnLocalTaskDef($taskDef, $outputFilePath) {
  $overridePath = "$HOME\JobNimbus\local-overrides.json"
  jnlocal dotenv --source taskdef $taskDef -o $outputFilePath -w $overridePath
  Write-Host "Secrets stored for $taskDef in $outputFilePath"
}
function runJnLocalLambdaName($lambdaName, $outputFilePath) {
  $overridePath = "$HOME\JobNimbus\local-overrides.json"
  jnlocal dotenv --source lambda $lambdaName -o $outputFilePath -w $overridePath
  Write-Host "Secrets stored for $lambdaName in $outputFilePath"
}

function createBranch($ticketNumber, $branchName) {
  git cob "danny/${ticketNumber}_${branchName}"
}

function test-jncore(){
  $env:env = "dev"
  yarn mocha --ui bdd -R spec -t 5000 '{,!(node_modules)/**}/*.spec.js'
}

function customFieldsPasswordDev(){
  $DBPORT="5432"
  $DB_USERNAME="phoenix"
  $DBNAME="app_db"
  $RDSHOST="custom-fields-dev-db.ce0vwjvstvmb.us-west-2.rds.amazonaws.com"
  echo "$(aws rds generate-db-auth-token --hostname $RDSHOST --port $DBPORT --username $DB_USERNAME)"
}

function customFieldsPasswordProd(){
  $DBPORT="5432"
  $DB_USERNAME="phoenix"
  $DBNAME="app_db"
  $RDSHOST="custom-fields-prod-db.cj67xhhcbjuh.us-east-1.rds.amazonaws.com"
  echo "$(aws rds generate-db-auth-token --hostname $RDSHOST --port $DBPORT --username $DB_USERNAME)"
}
