# machine-configs
This repository has morphed over the year in purpose. It's now intended to be used with the [Configurator](https://github.com/dannydwarren/configurator) which used to be embedded in this repository.

## Configurator project relocated
In order to support the Configurator as a stand-alone CLI project which can use any manifest provided I have split this repository, as the manifest, and the [Configurator](https://github.com/dannydwarren/configurator) into its own repository, as the CLI.

## Lessons Learned
The idea of having a simple JSON manifest file has proven insufficient. Some apps need additional files for configuring. This is especially true regarding backing up apps and the restoring them later. The backups need a place to live long term that supports versioning. Git is the current target so this repository will turn into only a manifest repository. App backups, which exclude secrets, will be included here where versioning will be supported by default.