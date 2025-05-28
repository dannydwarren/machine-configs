#!/bin/bash

#First Time Setup: https://jobnimbus.atlassian.net/wiki/spaces/DEV/pages/2054684772/Setup+AWS+CLI
#OPTIONS: aws configure list-profiles
function setAwsProfile() {
  local awsProfile=$1
  aws s3 ls --profile $awsProfile
  setAwsEnv $awsProfile
}
function loginAws() {
  local awsProfile=$1
  aws sso login --profile $awsProfile
  setAwsEnv $awsProfile
  setTerraformEnv $awsProfile
}
function setAwsEnv () {
  local awsProfile=$1
  export AWS_PROFILE=$awsProfile
  echo "AWS_PROFILE set to $AWS_PROFILE"
  export AWS_REGION=$(aws configure get region --profile $AWS_PROFILE)
  echo "AWS_REGION set to $AWS_REGION"
}
function setTerraformEnv() {
  local awsProfile=$1
  if( [ "$awsProfile" = "dev-terraform" ] ); then
    export TF_CLI_CONFIG_FILE=~/.terraformrc
    echo "TF_CLI_CONFIG_FILE set to $TF_CLI_CONFIG_FILE"

    eval $(jnlocal tfenv)

    echo "TF_VAR_aws_account_name set to $TF_VAR_aws_account_name"
    echo "TF_VAR_env set to $TF_VAR_env"
    echo "TF_VAR_runtime_config set to $TF_VAR_runtime_config"
    echo "TF_VAR_dd_app_key set to $TF_VAR_dd_app_key"
    echo "TF_VAR_dd_api_key set to $TF_VAR_dd_api_key"
  fi
}
