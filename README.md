# XMLDifferenceTool

Compare two XML files side by side and see exactly what is the same, what is different, and what exists in only one file.

[**Live demo**](https://lively-island-01761f910.1.azurestaticapps.net/) (runs in your browser) · [**Download the Windows installer**](https://github.com/koneill123/XML-Difference-Tool/releases/latest) · [License](LICENSE)

<!-- TODO: add a screenshot of a comparison, e.g. save it as docs/screenshot.png and uncomment:
![XMLDifferenceTool comparing two XML files](docs/screenshot.png) -->

## What it does

Load two XML files and the tool lays out their structure side by side, marking every element and attribute as:

- **Same**: present in both files with the same value
- **Different**: present in both files, but the values differ
- **Missing**: present in one file only (the other side shows an empty placeholder so the rows stay aligned)

It comes in two forms that share one comparison engine:

- **Windows desktop app** (WinForms), the main interface
- **Web app** (Blazor WebAssembly), which runs entirely in the browser. Your files are read locally and are not uploaded anywhere.

## Try it

- **In the browser:** open the [live demo](https://lively-island-01761f910.1.azurestaticapps.net/), choose two XML files, and press **Compare**.
- **On Windows:** download the `.msi` from the [Releases](https://github.com/koneill123/XML-Difference-Tool/releases) page. It installs for the current user (no administrator rights), includes the .NET runtime, and offers an optional desktop shortcut. The installer is not code-signed, so Windows SmartScreen may warn about an unknown publisher.
- **Sample files:** the [`Samples`](Samples) folder has two small files that differ, so you can see a comparison straight away.

## Features

- Node-by-node comparison of elements and attributes, with line numbers from the source files
- **Two ways to match repeated elements:** by their order in the file, or by name and value
- **Namespace handling** for files that use prefixes such as `xlink:` without declaring them (see below)
- Filters to show or hide same, different, and missing items, attributes, and elements, plus expand all and collapse all
- A progress indicator for large files, shown only if loading or comparing takes a noticeable moment
- Your display options are remembered between sessions (a settings file on Windows, browser storage on the web)
- Both front ends use the same shared library, so they give the same results

## Namespace handling

Real-world XML, such as hand-edited SVG, sometimes uses a prefix like `xlink:` without declaring it. Strict parsers reject those files. The **Namespace handling** option controls what happens:

| Mode | Behavior |
|------|----------|
| **Standard** | Normal XML namespace rules. A file that uses an undeclared prefix is rejected, with a hint about the option. |
| **Literal prefixes** | A prefix is treated as part of the name, so `xlink:href` is simply a name. Nothing in the file is changed. |
| **Automatic** (default) | Standard first. If a file fails because of a namespace problem, it is loaded with literal prefixes and a notice says so. If only one of the two files needed this, the other is loaded the same way so the names match. |

In the trees, names are shown with their prefix as written in the file (`xlink:href`, `xmlns:xlink`).

## Known limitations

- Files nested more than 500 levels deep are rejected with a message, to avoid crashing the app.
- Element and attribute names are case-sensitive, as XML requires: `<Item>` and `<item>` are different elements.
- In Standard mode, elements are matched by their local name, so two elements with the same local name in different namespaces count as the same element.
- Comments and processing instructions are not compared.

## Build from source

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download). The WinForms app needs Windows. [Visual Studio 2026](https://visualstudio.microsoft.com/) with the ".NET desktop development" workload is recommended, but the SDK alone is enough.

```
git clone https://github.com/koneill123/XML-Difference-Tool.git
cd XML-Difference-Tool
dotnet build XMLDifferenceTool.sln
```

Run the desktop app:

```
dotnet run --project src/XMLDifferenceTool
```

Run the web app, then open <http://localhost:5132>:

```
dotnet run --project src/XMLDifferenceTool.Web
```

## Building the installer and releases

The Windows installer is an MSI built with [WiX Toolset](https://wixtoolset.org/) v7 (per-user, 64-bit, self-contained). To build it locally:

```
.\installer\build-installer.ps1
```

WiX 7 asks you to accept its Open Source Maintenance Fee EULA on first use; see <https://docs.firegiant.com/wix/osmf/>.

To publish a release:

1. Set `VersionPrefix` in `Directory.Build.props` (add `VersionSuffix` for a pre-release) and commit.
2. Tag the commit: `git tag v1.2.3`, then `git push origin v1.2.3`.
3. A GitHub Actions workflow builds the installer and creates a **draft** release with the MSI and a SHA-256 checksum. Review it on GitHub and click Publish.

The workflow stops if the tag does not match the version built into the app.

## Solution layout

| Path | Description |
|------|-------------|
| `src/XMLDifferenceTool` | WinForms desktop application |
| `src/XMLDifferenceTool.Core` | Shared library: XML loading, the node model, and the diff engine |
| `src/XMLDifferenceTool.Web` | Blazor WebAssembly front end |
| `installer/` | WiX installer project and the build script |
| `Samples/` | Example XML files |
| `.github/workflows/` | Release and web deployment workflows |
| `Directory.Build.props` | The version number shared by all projects |

## License

Released under the [MIT License](LICENSE).

Copyright © 2026 Keith O'Neill
