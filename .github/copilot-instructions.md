## Purpose

This repository generates human-readable HTML documentation for Microsoft SQL Server databases (2005+).
This file gives pragmatic, repo-specific guidance for an AI coding assistant to be productive quickly.

## Quick Tasks

- **Build:** run `dotnet build` at the repository root (solution file `SqlServerDatabaseDocumentationGenerator.sln`). VS2019/VS2022 can also be used for .NET Framework builds.
- **Run Console Example:** edit `DocumentationGeneratorConsole/Program.cs` to change the connection string, then run the Console project; it writes `database.html` to the current directory.
- **Run GUI:** `DocumentationGeneratorApplication` is a WinForms app (MainForm.cs). Launch from Visual Studio to use the interactive connection-string dialog.

## Key Components (big picture)

- **Entry projects:**
  - `DocumentationGeneratorApplication/` — WinForms GUI, interactive connection string and save dialog (MainForm.cs, FrmConnectionString.cs).
  - `DocumentationGeneratorConsole/` — small console runner (Program.cs), currently uses a hard-coded `connStr` and writes `database.html`.
  - `SqlServerDatabaseDocumentationGenerator/` — core library: Document, Inspection, Model, Utility, and DesignIssue folders.

- **Core flow:**
  1. `DatabaseInspector` (in `Inspection/`) reads metadata from SQL Server (schemas, tables, views, columns, routines) into Model objects.
  2. `DatabaseHtmlDocumentGenerator` (in `Document/`) converts `Model.Database` to an HTML document using an internal `HtmlTextWriter` fallback.
  3. Output is an HTML file (Bootstrap CSS included inline).

## Repo-specific patterns & conventions

- Database object descriptions are pulled from the extended property named `MS_Description` (expect code to refer to this).
- The `Document/DatabaseHtmlDocumentGenerator.cs` contains a self-contained fallback for `System.Web.UI.HtmlTextWriter` — careful when adding System.Web references or testing on environments without it.
- Inspector classes follow the pattern `*Inspector.cs` (e.g., `TableInspector.cs`, `SchemaInspector.cs`) and populate POCOs in `Model/` — prefer working through the inspector methods to change how metadata is collected.

## Important files to inspect/edit

- `SqlServerDatabaseDocumentationGenerator/Document/DatabaseHtmlDocumentGenerator.cs` — main HTML generation; contains the app version and base CSS.
- `SqlServerDatabaseDocumentationGenerator/Inspection/DatabaseInspector.cs` — orchestrates other inspectors and exposes `GetDatabaseMetaData()`.
- `DocumentationGeneratorConsole/Program.cs` — minimal runnable example; useful for automation and CI.
- `DocumentationGeneratorApplication/` — UI-specific code (WinForms): `MainForm.cs`, `FrmConnectionString.cs`.
- `SqlServerDatabaseDocumentationGenerator.sln` and `.vscode/tasks.json` (task `build`) — canonical build targets.

## Building, testing, and debugging notes

- The solution targets .NET Framework 4.8 (Readme states 4.8, compatible with 4.5+). Use Visual Studio for WinForms debugging.
- To build from terminal (cross-machine):

```
dotnet build "SqlServerDatabaseDocumentationGenerator.sln"
```

- The console project currently hardcodes a connection string — update `Program.cs` or add CLI arg parsing before running in non-dev environments.
- Output HTML (example) is at `AdventureWorks-example.html` in the repo root — use it as a sample for layout and expected content.

## Integration & external dependencies

- Expects a reachable Microsoft SQL Server instance and valid connection string (Integrated or SQL auth).
- Third-party code lives under `third-party/` (e.g., `BeTimvwFramework`); inspect those for license/compat issues.

## Coding guidance for PRs

- Preserve public model shapes in `Model/` where possible — many components (inspectors, document generator) rely on these POCOs.
- When modifying HTML output, edit `DatabaseHtmlDocumentGenerator.cs`. There is an inline `baseCss` and bootstrap usage; search for `baseCss` in that file.
- Avoid introducing a hard dependency on `System.Web` — the repo intentionally includes a lightweight fallback. If you add System.Web references, update project files and ensure builds still succeed on CI/targets.

## Helpful searches/examples

- To find metadata collection code: search for `GetDatabaseMetaData`, `DatabaseInspector`, or names under `Inspection/`.
- To find where descriptions are read: search for `MS_Description` or `ExtendedProperty` string patterns.

## If you need more

If any part of the flow or environment is unclear (e.g., which project is used in a specific CI or how tests are run), tell me which area to expand and I will update this file with concrete commands and examples.
