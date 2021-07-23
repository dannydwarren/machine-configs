Copy-Item $PSScriptRoot\..\programs\Everything.ini (scoop prefix everything)
everything -install-run-on-system-startup
everything -startup
