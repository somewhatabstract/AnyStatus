# AnyStatus
Monitor the health, status and availability of services

[![Build status](https://ci.appveyor.com/api/projects/status/t7j7rnnci6lvv8jl?svg=true)](https://ci.appveyor.com/project/AlonAmsalem/anystatus)

AnyStatus is a free and open source Visual Studio extension that adds basic monitoring capabilities to Visual Studio for resources and applications you run on your development and other environments. AnyStatus can monitor Builds (Jenkins, TeamCity & AppVeyor), Ping, TCP and HTTP response status code. More status and health checks will be added in future versions. AnyStatus runs in the background and does not have a significant impact on the performance or startup of Visual Studio.

**Note:** This is an alpha version that comes with an incomplete set of basic features and has some known issues. I will be constantly working on AnyStatus to make sure every bug is fixed and all designed features are implemented.

### Features 

* Organize health checks in folders
* Colored status indicators
* Monitor Jenkins, TeamCity and AppVeyor builds
* Monitor HTTP response status codes
* Monitor Ping requests
* Monitor TCP ports

If you wish for other types of status and health checks, please write a review or create a new issue on GitHub.

### License

[Apache 2.0](https://github.com/AlonAm/AnyStatus/blob/master/LICENSE)

### Roadmap

* TFS & VSO build status
* Travis CI build status
* Start/Cancel build
* Visual Studio theme support

### Release Notes

**0.5**
- Added AppVoyer build status
- Edit health check properties
- Enable/Disable health check
- Refresh all health checks toolbar button
- UI improvements

**0.4**
- Fixed a critical bug in New Item Wizard
- Added TeamCity build status
- Visual Studio 2013 Support

**0.3**
- Added a Refresh button to health check context menu
- Added TCP port health check

**0.2**
- Added Ping health check

**0.1**
- Initial alpha release


