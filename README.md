# SuperUnityBuild - BuildActions
> BuildActions for use with the [SuperUnityBuild](unitybuild) build automation tool.

BuildActions are one of the key components that make the [SuperUnityBuild][unitybuild] build automation tool flexible and powerful. Each one extends the capabilities of SuperUnityBuild and can be easily setup and customized to suit your needs, and if there's some other specialized functionality that your project needs, you can simply write your own BuildActions and integrate them into the build process.

This repository is intended to serve as an ever expanding library of useful BuildActions as more of them are created.

## Basic Usage

If you don't already have SuperUnityBuild, get it from its [GitHub repository][unitybuild]. Then, do one of the following:

* [Download][download] this project and copy the folder for the tools you want into your project's Assets directory.
* Make this repository a git submodlue within your project's Assets directory.

Refer to the [SuperUnityBuild wiki][unitybuild-wiki] for basic information on how to setup, use, and create your own BuildActions. For specific details on one of the BuildActions featured here, refer to the README in its directory.

## Tools

* [**AssetBundle Builder**][assetbundle]
* **Batch File Runner**
* [**Zip File Utility**][zipfile]
* **File Operations Utility**
* [**itch.io Butler Upload**][itchuploader]


[download]: https://github.com/Chaser324/unity-build-actions/archive/master.zip
[unitybuild]: https://github.com/Chaser324/unity-build
[unitybuild-wiki]: https://github.com/Chaser324/unity-build/wiki/Build-Actions

[assetbundle]: https://github.com/Chaser324/unity-build-actions/tree/master/UnityBuild-AssetBundle
[zipfile]: https://github.com/Chaser324/unity-build-actions/tree/master/UnityBuild-ZipFile
[itchuploader]: https://github.com/Chaser324/unity-build-actions/tree/master/UnityBuild-ItchUploader
