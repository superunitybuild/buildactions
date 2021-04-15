# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

<!-- ## [Unreleased] -->

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

[unreleased]: https://github.com/superunitybuild/buildactions/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/superunitybuild/buildactions/compare/5951d33...v1.0.0
