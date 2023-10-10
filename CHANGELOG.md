# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- ## [Unreleased] -->

## [4.0.1] - 2023-10-10

### Fixed

-   Fixed output file name not being parsed for string tokens in `ZipFileOperation` action. (by [@cosformula](https://github.com/cosformula))

## [4.0.0] - 2023-10-03

### Added

-   Added support for new Per-Build and Single Run string tokens. Per-Build actions have gained support for `$BASEPATH` and `$BUILDPATH`, whilst Single Run actions can now use `$VERSION`, `$BUILD`, `$YEAR`, `$MONTH`, `$DAY` and `$TIME`, with the time-based tokens corresponding to the time at which the action was run.

### Changed

-   Standardised string token usage across Build Actions.

## [3.0.1] - 2023-05-23

### Fixed

-   Fixed minimum required SuperUnityBuild version.

## [3.0.0] - 2023-05-16

### Added

-   **Breaking change:** Added support for new per-build scripting backend functionality.
-   Added author details and documentation, changelog and license URLs to package manifest.

### Changed

-   **Breaking change:** Drop support for versions of Unity older than 2020.3 LTS.
-   Refactor `FileUtility` build action and fix exceptions when paths not configured.

## [2.2.0] - 2022-02-03

### Added

-   Added support for WebGL and UWP platforms to `ItchUploader` action.

### Fixed

-   Fixed `ItchUploader` action generating invalid butler command for unknown build platforms.

## [2.1.1] - 2021-10-07

### Fixed

-   Fixed incorrect selection in audio spatializer dropdown on startup in `PerBuildAudioSettings` action.

## [2.1.0] - 2021-08-24

### Added

-   Allow certain pre-build per-platform actions to optionally configure Editor.

### Changed

-   Removed pre-Unity 2019.1 code.

## [2.0.0] - 2021-05-13

### Changed

-   Increased minimum supported Unity version to 2019.1.

## [1.3.0] - 2021-05-10

### Changed

-   Updated `FileUtility` and `ItchUploader` build actions to use new `BuildSettings.productParameters.buildVersion` property that deprecated `BuildSettings.productParameters.lastGeneratedVersion`.

## [1.2.0] - 2021-04-29

### Added

-   Added `PerBuildAudioSettings` build action.

## [1.1.0] - 2021-04-20

### Changed

-   Removed pre-Unity 2018.1 code.
-   Disable `XRSettings` build action when XR Plug-in Management package is installed.

## [1.0.0] - 2021-04-15

This release includes all [previous commits][1.0.0] to the project, plus:

### Added

-   Added UPM support.
-   First tagged version.
-   Support use of `$VERSION` and `$BUILD` replacement tokens in single-run action paths in `FileUtility` action.
-   Improved error reporting in `FileUtility` action.
-   Added `$BASEPATH` to File and Folder path substitutions in `FileUtility` action. [PR #18](https://github.com/superunitybuild/buildactions/pull/18)
-   Added input to customize channel names to `ItchUploader` action [PR #14](https://github.com/superunitybuild/buildactions/pull/14)

### Changed

-   Support all versions of Unity XR Plug-in Management in `XRPluginManagement` action.

### Fixed

-   `OverrideDefines` action no longer ignore pre build defines. [PR #17](https://github.com/superunitybuild/buildactions/pull/17)

[unreleased]: https://github.com/superunitybuild/buildactions/compare/v4.0.1...HEAD
[4.0.1]: https://github.com/superunitybuild/buildactions/compare/v4.0.0...v4.0.1
[4.0.0]: https://github.com/superunitybuild/buildactions/compare/v3.0.1...v4.0.0
[3.0.1]: https://github.com/superunitybuild/buildactions/compare/v3.0.0...v3.0.1
[3.0.0]: https://github.com/superunitybuild/buildactions/compare/v2.2.0...v3.0.0
[2.2.0]: https://github.com/superunitybuild/buildactions/compare/v2.1.1...v2.2.0
[2.1.1]: https://github.com/superunitybuild/buildactions/compare/v2.1.0...v2.1.1
[2.1.0]: https://github.com/superunitybuild/buildactions/compare/v2.0.0...v2.1.0
[2.0.0]: https://github.com/superunitybuild/buildactions/compare/v1.3.0...v2.0.0
[1.3.0]: https://github.com/superunitybuild/buildactions/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/superunitybuild/buildactions/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/superunitybuild/buildactions/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/superunitybuild/buildactions/compare/5951d33...v1.0.0
