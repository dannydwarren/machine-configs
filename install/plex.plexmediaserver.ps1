function InstallPlugin($githubRepo) {
    git clone https://github.com/$githubRepo.git "$env:LOCALAPPDATA\Plex Media Server\Plug-ins\$($githubRepo.split('/')[1])"
}

# TODO: Do I want these plugins? Others?
#InstallPlugin gboudreau/XBMCnfoTVImporter.bundle
#InstallPlugin gboudreau/XBMCnfoMoviesImporter.bundle
