# SuperUnityBuild - BuildActions
> BuildActions for use with the [SuperUnityBuild][buildtool] build automation tool.

BuildActions are one of the key components that make the [SuperUnityBuild][buildtool] build automation tool flexible and powerful. Each one extends the capabilities of SuperUnityBuild and can be easily setup and customized to suit your needs, and if there's some other specialized functionality that your project needs, you can simply write your own BuildActions and integrate them into the build process.

This repository is intended to serve as an ever expanding library of useful BuildActions as more of them are created.

## Basic Usage

If you don't already have SuperUnityBuild, get it from its [GitHub repository][buildtool]. Then, do one of the following:

* [Download][download] this project and copy the folder for the tools you want into your project's Assets directory.
* Make this repository a git submodule within your project's Assets directory.

Refer to the [SuperUnityBuild wiki][unitybuild-wiki] for basic information on how to setup, use, and create your own BuildActions. For specific details on one of the BuildActions featured here, refer to the README in its directory.

## Tools

* [**AssetBundle Builder**][assetbundle] - AssetBundle building and file management.
* [**Script Runner**][scriptrunner] - Run batch/script files or other external applications.
* [**Zip File Utility**][zipfile] - Compress builds and other files into ZIP file format.
* [**File Operations Utility**][fileutil] - Perform basic file/folder operations like copy, move, and delete.
* [**itch.io Uploader**][itchuploader] - Upload builds to [itch.io](https://itch.io/).
* [**Define Modifier**][overridedefines] - Add/remove script defines.

## Contributing
Bug reports, feature requests, and pull requests are welcome and appreciated.

## Credits
* **Chase Pettit** - [github](https://github.com/Chaser324), [twitter](http://twitter.com/chasepettit)

## License
All code in this repository ([unity-build-actions](https://github.com/superunitybuild/buildactions)) is made freely available under the MIT license. This essentially means you're free to use it however you like as long as you provide attribution.

Zip File Utility includes a pre-compiled DotNetZip library which is licensed under the Ms-PL. See [DotNetZip's repository](https://dotnetzip.codeplex.com/) for more info.





[download]: https://github.com/superunitybuild/buildactions/archive/master.zip
[buildtool]: https://github.com/Chaser324/unity-build
[unitybuild-wiki]: https://github.com/Chaser324/unity-build/wiki/Build-Actions

[assetbundle]: https://github.com/superunitybuild/buildactions/tree/master/UnityBuild-AssetBundle
[fileutil]: https://github.com/superunitybuild/buildactions/tree/master/UnityBuild-FileUtility
[scriptrunner]: https://github.com/superunitybuild/buildactions/tree/master/UnityBuild-ScriptRunner
[zipfile]: https://github.com/superunitybuild/buildactions/tree/master/UnityBuild-ZipFile
[itchuploader]: https://github.com/superunitybuild/buildactions/tree/master/UnityBuild-ItchUploader
[overridedefines]: https://github.com/superunitybuild/buildactions/tree/master/UnityBuild-OverrideDefines
