# SuperUnityBuild BuildActions

[![openupm](https://img.shields.io/npm/v/com.github.superunitybuild.buildactions?label=openupm&registry_uri=https://package.openupm.com)][openupm-package]

> BuildActions for use with the [SuperUnityBuild][buildtool] build automation tool.

![Logo](https://raw.githubusercontent.com/superunitybuild/buildtool/gh-pages/Cover.png)

[Unity Forums Thread][unity-forums-thread] | [Documentation Wiki][wiki] | [OpenUPM package][openupm-package]

BuildActions are one of the key components that make the [SuperUnityBuild][buildtool] build automation tool flexible and powerful. Each one extends the capabilities of SuperUnityBuild and can be easily setup and customized to suit your needs, and if there's some other specialized functionality that your project needs, you can simply write your own BuildActions and integrate them into the build process.

This repository is intended to serve as an ever expanding library of useful BuildActions as more of them are created.

## Basic Usage

### Installation

BuildActions requires SuperUnityBuild to be [installed](https://github.com/superunitybuild/buildtool#installation) in your project.

Official releases of BuildActions can be installed via [Unity Package Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html) from the [OpenUPM](https://openupm.com) package registry. See [https://openupm.com/packages/com.github.superunitybuild.buildactions/][openupm-package] for installation options.

You can also [download the source zip][download] of this repository and extract its contents into your Unity project's `Packages` directory to install BuildActions as an embedded package.

### Setup

Refer to the [SuperUnityBuild wiki][wiki] for basic information on how to setup, use, and create your own BuildActions. For specific details on one of the BuildActions featured here, refer to the README in its directory.

## Tools

-   [**AssetBundle Builder**][assetbundle] - AssetBundle building and file management.
-   [**Script Runner**][scriptrunner] - Run batch/script files or other external applications.
-   [**Zip File Utility**][zipfile] - Compress builds and other files into ZIP file format.
-   [**File Operations Utility**][fileutil] - Perform basic file/folder operations like copy, move, and delete.
-   [**itch.io Uploader**][itchuploader] - Upload builds to [itch.io](https://itch.io/).
-   [**Override Defines**][overridedefines] - Add/remove script defines.
-   [**Package Management**][packagemanagement] - Enable per-build Unity Package Manager settings.
-   [**Per-Build Player Settings**][perbuildplayersettings] - Enable per-build Player settings.
-   [**XR Plug-in Management**][xrpluginmanagement] - Enable per-build [Unity XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@latest) settings.

## Contributing

Bug reports, feature requests, and pull requests are welcome and appreciated.

## Credits

### Creator

-   **Chase Pettit** - [GitHub](https://github.com/Chaser324), [Twitter](http://twitter.com/chasepettit)

### Maintainer

-   **Robin North** - [GitHub](https://github.com/robinnorth)

### Contributors

You can see a complete list of contributors at [https://github.com/superunitybuild/buildactions/graphs/contributors][contributors]

## License

All code in this repository ([buildactions](https://github.com/superunitybuild/buildactions)) is made freely available under the MIT license. This essentially means you're free to use it however you like as long as you provide attribution.

Zip File Utility includes a pre-compiled DotNetZip library which is licensed under the Ms-PL. See [DotNetZip's repository](https://dotnetzip.codeplex.com/) for more info.

[download]: https://github.com/superunitybuild/buildactions/archive/master.zip
[contributors]: https://github.com/superunitybuild/buildactions/graphs/contributors
[release]: https://github.com/superunitybuild/buildactions/releases
[buildtool]: https://github.com/superunitybuild/buildtool
[buildactions]: https://github.com/superunitybuild/buildactions
[wiki]: https://github.com/superunitybuild/buildtool/wiki/Build-Actions
[openupm-package]: https://openupm.com/packages/com.github.superunitybuild.buildactions/
[unity-forums-thread]: https://forum.unity3d.com/threads/super-unity-build-automated-build-tool-and-framework.471114/
[assetbundle]: https://github.com/superunitybuild/buildactions/tree/master/Editor/AssetBundle
[fileutil]: https://github.com/superunitybuild/buildactions/tree/master/Editor/FileUtility
[scriptrunner]: https://github.com/superunitybuild/buildactions/tree/master/Editor/ScriptRunner
[zipfile]: https://github.com/superunitybuild/buildactions/tree/master/Editor/ZipFile
[itchuploader]: https://github.com/superunitybuild/buildactions/tree/master/Editor/ItchUploader
[overridedefines]: https://github.com/superunitybuild/buildactions/tree/master/Editor/OverrideDefines
[packagemanagement]: https://github.com/superunitybuild/buildactions/tree/master/Editor/PackageManagement
[perbuildplayersettings]: https://github.com/superunitybuild/buildactions/tree/master/Editor/PerBuildPlayerSettings
[xrpluginmanagement]: https://github.com/superunitybuild/buildactions/tree/master/Editor/XRPluginManagement
