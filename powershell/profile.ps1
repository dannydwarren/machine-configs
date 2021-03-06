# TODO: REVIEW BELOW and migrate to /manifests/profile.ps1

function Update-WindowsTerminalSettings() {
    Import-Module Appx -UseWindowsPowerShell
    Copy-Item $PSScriptRoot\settings.json "$env:LocalAppData\Packages\$((Get-AppxPackage -Name Microsoft.WindowsTerminal).PackageFamilyName)\LocalState\settings.json"
}

function Test-IsAdmin() {
    ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Run-AsAdmin([Parameter(Mandatory)][string]$FilePath) {
    Start-Process wt -Verb RunAs -ArgumentList "-w run-as-admin pwsh -File `"$FilePath`""
}

function Create-Shortcut([Parameter(Mandatory)][string]$Target, [Parameter(Mandatory)][string]$Link, [string]$Arguments) {
    $shortcut = (New-Object -ComObject WScript.Shell).CreateShortcut($Link)
    $shortcut.TargetPath = $Target
    $shortcut.WorkingDirectory = Split-Path $Target
    $shortcut.Arguments = $Arguments
    $shortcut.Save()
}

function Create-RunOnce([Parameter(Mandatory)][string]$Description, [Parameter(Mandatory)][string]$Command) {
    # https://docs.microsoft.com/en-us/windows/win32/setupapi/run-and-runonce-registry-keys
    Set-RegistryValue "HKCU:\Software\Microsoft\Windows\CurrentVersion\RunOnce" -Name $Description -Value $Command
}

function Create-FileRunOnce([Parameter(Mandatory)][string]$Description, [Parameter(Mandatory)][string]$FilePath) {
    Create-RunOnce $Description "$((Get-Command wt).Source) -w run-once pwsh -Command `"Run-AsAdmin '$FilePath'`""
}

function Get-TimestampForFileName() {
    (Get-Date -Format o) -replace ":", "_"
}

function Get-SafeFileName([Parameter(Mandatory)][string]$FileName) {
    $invalidFileNameChars = [Regex]::Escape([IO.Path]::GetInvalidFileNameChars() -join "")
    $FileName -replace "[$invalidFileNameChars]", ""
}

function Set-RegistryValue([Parameter(Mandatory)][string]$Path, [string]$Name = "(Default)", [Parameter(Mandatory)][object]$Value) {
    if (!(Test-Path $Path)) {
        New-Item $Path -Force | Out-Null
    }
    Set-ItemProperty $Path -Name $Name -Value $Value
}

function Set-EnvironmentVariable([Parameter(Mandatory)][string]$Variable, [string]$Value) {
    [Environment]::SetEnvironmentVariable($Variable, $Value, "User")
    [Environment]::SetEnvironmentVariable($Variable, $Value, "Process")
}

function Get-ProgramsInstalled() {
    return (Get-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*).DisplayName +
    (Get-ItemProperty HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*).DisplayName +
    (Get-ItemProperty HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*).DisplayName |
    sort
}

function Test-ProgramInstalled([Parameter(Mandatory)][string]$NameLike) {
    return (Get-ProgramsInstalled) -like "*$NameLike*"
}

function SecureRead-Host([string]$Prompt) {
    $secureString = Read-Host -Prompt $Prompt -AsSecureString
    $binaryString = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($secureString)
    $string = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($binaryString)
    return $string
}

function Download-File([Parameter(Mandatory)][string]$Uri, [Parameter(Mandatory)][string]$OutFile, [switch]$AutoDetermineExtension) {
    $savedProgressPreference = $ProgressPreference
    $ProgressPreference = "SilentlyContinue"
    if ($AutoDetermineExtension) {
        $contentType = (Invoke-WebRequest $Uri -Method Head).Headers["Content-Type"]
        $ext = switch ($contentType) {
            "image/jpeg" { ".jpg" }
            "image/png" { ".png" }
            Default { throw "Unknown content type `"$contentType`"" }
        }
        $OutFile += $ext
    }
    Write-Host "Downloading $Uri`n`tto $OutFile"
    $downloadFolder = Split-Path $OutFile
    if (!(Test-Path $downloadFolder)) {
        New-Item $downloadFolder -ItemType Directory -Force | Out-Null
    }
    Invoke-WebRequest $Uri -OutFile $OutFile -MaximumRetryCount 3 -RetryIntervalSec 30
    $ProgressPreference = $savedProgressPreference
}

function AddNuGetSource([Parameter(Mandatory)][string]$Name, [Parameter(Mandatory)][string]$Path) {
    $nugetConfigPath = "$env:AppData\NuGet\nuget.config"
    if (!(Select-String $Path $nugetConfigPath)) {
        [xml]$nugetConfigXml = Get-Content $nugetConfigPath
        $newPackageSource = $nugetConfigXml.CreateElement("add")
        $newPackageSource.SetAttribute("key", $Name)
        $newPackageSource.SetAttribute("value", $Path)
        $nugetConfigXml.configuration.packageSources.AppendChild($newPackageSource)
        $nugetConfigXml.Save($nugetConfigPath)
    }
}

function Find-RepoRoot() {
    $repoRoot = Get-Location
    while (!(Test-Path $repoRoot\.git)) {
        if ($repoRoot -like "*:\") {
            throw "No git repo found between $pwd and $repoRoot"
        }
        $repoRoot = Resolve-Path "$repoRoot\.."
    }
    return $repoRoot
}

function GitAudit() {
    function CheckDir($dir) {
        pushd $dir
        if (Test-Path (Join-Path $dir .git)) {
            $unsynced = git unsynced
            $status = git status --porcelain
            if ($unsynced -or $status) {
                Write-Output (New-Object System.String -ArgumentList ('*', 100))
                Write-Host $dir -ForegroundColor Red
                git unsynced
                git status --porcelain
            }
        }
        popd
    }
    (Get-ChildItem $src) +
    (Get-ChildItem C:\Work | Get-ChildItem) |
    % { CheckDir $_.FullName }
}

function ReallyUpdate-Module([Parameter(Mandatory)][string]$Name) {
    Update-Module $Name -Force

    Get-Module $Name -ListAvailable |
    sort Version -Descending |
    select -Skip 1 |
    % { Uninstall-Module $Name -RequiredVersion $_.Version }
}

function winget-manifest([Parameter(Mandatory)][string]$AppId) {
    $shardLetter = $AppId.ToLower()[0]
    $path = $AppId -replace "\.", "/"
    $version = (winget show $AppId | sls "(?<=Version: ).*").Matches.Value
    $manifestUrl = "https://raw.githubusercontent.com/microsoft/winget-pkgs/master/manifests/$shardLetter/$path/$version/$AppId.installer.yaml"
    Write-Output "Fetching from $manifestUrl"
    try {
        $response = iwr $manifestUrl
    }
    catch {
        $manifestUrl = "https://raw.githubusercontent.com/microsoft/winget-pkgs/master/manifests/$shardLetter/$path/$version/$AppId.yaml"
        Write-Output "Fetching from $manifestUrl"
        $response = iwr $manifestUrl
    }
    Write-Output $response.Content
}

Register-ArgumentCompleter -Native -CommandName .\config.ps1 -ScriptBlock {
    param($wordToComplete, $commandAst, $cursorPosition)
    if ((Get-Location).Path -ne "$src\configs") {
        return
    }

    dir $src\configs *.ps1 -Recurse |
    sls "Block `"(.+?)`"" |
    % { "`"$($_.Matches.Groups[1].Value)`"" } |
    ? { $_ -like "*$wordToComplete*" } |
    sort |
    % {
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }
}

function tmpfor([Parameter(Mandatory)][string]$For) {
    "$tmpToDelete\$($For)_$(Get-TimestampForFileName)"
}

function togh([Parameter(Mandatory)][string]$FilePath, [int]$BeginLine, [int]$EndLine) {
    if ($FilePath -notmatch "C:\\Work\\(?<org>[^\\]+)\\(?<repo>[^\\]+)") {
        Write-Error "Could not match path"
    }

    pushd $Matches[0]
    $permalinkCommit = git rev-parse --short head
    popd

    $url = ($FilePath.Replace($Matches[0], "https://github.com/$($Matches["org"])/$($Matches["repo"])/blob/$permalinkCommit") -replace "\\", "/") `
        + ($BeginLine -gt 0 ? "#L$BeginLine" + ($EndLine -gt 0 ? "-L$EndLine" : "") : "")

    Set-Clipboard $url
    Write-Host "$url`n`tadded to clipboard"
}

. $PSScriptRoot\one-liners.ps1

$transcriptDir = "C:\BenLocal\PowerShell Transcripts"
Get-ChildItem "$transcriptDir\*.log" | ? { !(sls -Path $_ -Pattern "Command start time:" -SimpleMatch -Quiet) } | rm -ErrorAction SilentlyContinue
$Transcript = "$transcriptDir\$(Get-TimestampForFileName).log"
Start-Transcript $Transcript -NoClobber -IncludeInvocationHeader

Set-PoshPrompt $PSScriptRoot\ben.omp.json
