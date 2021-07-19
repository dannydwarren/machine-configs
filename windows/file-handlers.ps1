Block "Associate extensionless files with VS Code" {
    & "$PSScriptRoot\associate-extensionless-files-with-vs-code.ps1"
}

Block "Create generic file handler" {
    Set-RegistryValue "HKLM:\SOFTWARE\Classes\Ben.VSCode\shell\open\command" -Value """$env:LocalAppData\Programs\Microsoft VS Code\Code.exe"" ""%1"""
}

function CreateFileHandlerBlock([string]$Handler, [string]$DisplayName, [string]$DefaultIcon, [string]$OpenCommand) {
    Block "Create file handler $Handler" {
        Set-RegistryValue "HKCU:\SOFTWARE\Classes\$Handler" -Value $DisplayName
        Set-RegistryValue "HKCU:\SOFTWARE\Classes\$Handler\DefaultIcon" -Value $DefaultIcon
        Set-RegistryValue "HKCU:\SOFTWARE\Classes\$Handler\shell\open\command" -Value $OpenCommand
    }
}

function AssociateFileBlock([string]$Extension, [string]$Handler) {
    Block "Associate $Extension with $Handler" {
        Set-RegistryValue "HKCU:\SOFTWARE\Classes\.$($Extension.Trim('.'))" -Value $Handler
    }
}

function 7ZipFileBlock([string]$Extension, [int]$IconIndex) {
    $trimmedExtension = $Extension.Trim('.')
    $handler = "7-Zip.$trimmedExtension"
    CreateFileHandlerBlock $handler "$trimmedExtension Archive" "$(scoop prefix 7zip)\7z.dll,$IconIndex" """$(scoop prefix 7zip)\7zFM.exe"" ""%1"""
    AssociateFileBlock $Extension $handler
}

AssociateFileBlock txt VSCodeSourceFile
AssociateFileBlock log VSCodeSourceFile
AssociateFileBlock log1 VSCodeSourceFile
AssociateFileBlock log2 VSCodeSourceFile
AssociateFileBlock log3 VSCodeSourceFile
AssociateFileBlock log4 VSCodeSourceFile
AssociateFileBlock log5 VSCodeSourceFile
AssociateFileBlock log6 VSCodeSourceFile
AssociateFileBlock log7 VSCodeSourceFile
AssociateFileBlock log8 VSCodeSourceFile
AssociateFileBlock log9 VSCodeSourceFile
AssociateFileBlock log10 VSCodeSourceFile
AssociateFileBlock ps1 VSCodeSourceFile
AssociateFileBlock ini VSCodeSourceFile
AssociateFileBlock json VSCodeSourceFile
AssociateFileBlock xml VSCodeSourceFile
AssociateFileBlock config VSCodeSourceFile
AssociateFileBlock DotSettings VSCodeSourceFile
AssociateFileBlock creds VSCodeSourceFile
AssociateFileBlock pgpass VSCodeSourceFile
AssociateFileBlock yarnrc VSCodeSourceFile
AssociateFileBlock nvmrc VSCodeSourceFile
AssociateFileBlock csx VSCodeSourceFile
AssociateFileBlock cs VSCodeSourceFile
AssociateFileBlock fsx VSCodeSourceFile
AssociateFileBlock fs VSCodeSourceFile
AssociateFileBlock java VSCodeSourceFile

# TODO: Ask Ben
Block "Set .ahk Edit command to VS Code" {
    Set-RegistryValue "HKLM:\SOFTWARE\Classes\AutoHotkeyScript\Shell\Edit\Command" -Value "code.exe %1"
}

7ZipFileBlock 7z 0
7ZipFileBlock zip 1
7ZipFileBlock rar 3
7ZipFileBlock cab 7
7ZipFileBlock tar 13
7ZipFileBlock gz 14
7ZipFileBlock gzip 14