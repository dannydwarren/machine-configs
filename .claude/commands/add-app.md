# Add App Command

Add a new app configuration to machine-configs.

## Required Information

The user must provide:
- **AppId**: Unique identifier (e.g., "Microsoft.DotNet.SDK.10", "mob")
- **AppType**: One of: winget, scoop, scoopBucket, GitRepo, PowerShellModule, PowerShell, Script, Gitconfig
- **Environments**: Target environments (e.g., "All", "dev", "JobNimbus", "dev|JobNimbus")

## Optional Information

- **InstallArgs**: Installation arguments (required for GitRepo - the git URL)
- **PreventUpgrade**: Set to true to prevent upgrades
- **Notes**: Notes about the app
- **FolderName**: Custom folder name (defaults to AppId)

## User Input

$ARGUMENTS

## Instructions

1. Parse the user input to extract the required fields (AppId, AppType, Environments) and any optional fields.

2. If any required fields are missing, ask the user to provide them before proceeding.

3. If AppType is "GitRepo" and InstallArgs is not provided, ask for the git URL.

4. Create the app directory at `apps/<FolderName>/` (use AppId if FolderName not specified).

5. Create `apps/<FolderName>/app.json` with the following structure:
   ```json
   {
     "appId": "<AppId>",
     "appType": "<AppType>",
     "environments": "<Environments>"
   }
   ```
   Add optional fields (installArgs, preventUpgrade, notes) only if provided.

6. Read `manifest.json` and add the folder name to the "apps" array if not already present.

7. Report what was created.

## Examples

- `/add-app Microsoft.DotNet.SDK.10 winget dev|JobNimbus`
- `/add-app mob scoop All`
- `/add-app git.JobNimbus.myrepo GitRepo JobNimbus https://github.com/JobNimbus/myrepo.git`
