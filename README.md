# SolidConvert Pro

**SolidConvert Pro** is a Windows desktop application for batch-exporting **SOLIDWORKS drawings** (`.slddrw`) to **PDF** and/or **DWG**. It connects to SOLIDWORKS through the official COM API, queues multiple files, shows per-file status, and supports cancellation mid-batch.

---

## Features

- **Batch conversion** — Add many `.slddrw` files and export them in one run.
- **Dual format export** — Choose **PDF**, **DWG**, or both (at least one must be selected).
- **Flexible file input** — Use **Add Files** (multi-select dialog) or **drag and drop** `.slddrw` onto the window.
- **Output folder** — Browse to a destination folder; the last folder is **remembered** between sessions (user settings).
- **Progress UI** — Status text, current file name, progress bar, and **N / total** with percentage.
- **Per-file status** — Visual indicators for pending, processing, completed, and error; error messages appear in the list when a file fails.
- **Cancel** — Stop the remaining queue after the current file finishes its COM work (cooperative cancellation).
- **Revision-aware naming** — Output names include a **Revision** value from the drawing’s custom properties when present (see [Output naming](#output-naming)).

---

## Requirements

| Requirement | Notes |
|-------------|--------|
| **OS** | Windows (WPF desktop app). |
| **.NET Framework** | **4.7.2** (see `App.config` / project target). |
| **SOLIDWORKS** | Must be **installed** and licensed on the machine. The app attaches to a **running** SOLIDWORKS instance when possible; otherwise it **starts a new visible** session. |
| **Visual Studio** | **2019 or later** recommended (solution format targets VS 17+). Any edition that supports .NET Framework 4.7.2 and WPF. |

### SOLIDWORKS API / interop version

The project references **SOLIDWORKS interop assemblies** shipped via the **Xarial.XCad.SolidWorks** NuGet package (see `packages.config`). Interop versions are tied to a specific SOLIDWORKS major release. If you use a **different** SOLIDWORKS year than the one the interops target, you may need to:

- Update NuGet packages to versions that match your installation, or  
- Reference interop DLLs from your local SOLIDWORKS API redistributable / installation.

Build failures or runtime COM errors after a SOLIDWORKS upgrade often indicate a **interop mismatch**.

---

## Getting started

### 1. Clone the repository

```powershell
git clone https://github.com/Urjitpatel28/SolidConvert-Pro.git
cd SolidConvert-Pro
```

(Adjust the URL if your remote differs.)

### 2. Restore NuGet packages

From the folder containing `SolidConvert Pro.sln`:

```powershell
nuget restore "SolidConvert Pro.sln"
```

Or open the solution in Visual Studio and use **Restore NuGet Packages**.

### 3. Build

- Open **`SolidConvert Pro.sln`** in Visual Studio.  
- Select configuration **Debug** or **Release** and platform **Any CPU**.  
- **Build → Build Solution** (or `Ctrl+Shift+B`).

Output:

- **Debug:** `bin\Debug\`  
- **Release:** `bin\Release\`

Main executable: **`SolidConvert Pro.exe`**

### 4. Command-line build (optional)

If MSBuild is on your PATH:

```powershell
msbuild "SolidConvert Pro.sln" /p:Configuration=Release
```

---

## How to use

1. **Start SOLIDWORKS** (optional but recommended). If it is not running, the app will attempt to launch it.
2. Run **`SolidConvert Pro.exe`**.
3. Under **Export Format**, check **Export to PDF** and/or **Export to DWG**.
4. Add drawings:
   - Click **Add Files** and select one or more `.slddrw` files, **or**
   - Drag `.slddrw` files onto the window.
5. Click **Browse** under **Output Folder** and pick where exports should be written.
6. Click **Export**. Watch **Progress** and the file list for status.
7. Use **Cancel** during a run to stop after the current file.

**Remove Selected** / **Clear List** help manage the queue; they are disabled while a conversion is in progress.

---

## Output naming

Behavior is implemented in `SolidWorksService`:

- **Revision** is read from the drawing custom property named **`Revision`**. If it is missing or empty, the app uses **`R0`**.
- **PDF** (when enabled):  
  `{OriginalFileName}_{Revision}.pdf`  
  Example: `Bracket_A.slddrw` → `Bracket_A_REV-A.pdf` if `Revision` is `REV-A`.
- **DWG** (when enabled):  
  - **Single-sheet** drawing: `{OriginalFileName}_{Revision}.dwg`  
  - **Multi-sheet** drawing: one DWG per sheet — `{OriginalFileName}_{Revision}-1.dwg`, `-2.dwg`, …

PDF export uses SOLIDWORKS export data with **“view PDF after saving”** turned off. For DWG, the code applies **DXF/DWG-related user preferences** at connect time (e.g. DXF version **R2000**, fonts and line styles output, multi-sheet DXF option set to **active sheet only** — see `SolidWorksService.ConnectAsync` for the exact preferences).

---

## Architecture (high level)

| Area | Role |
|------|------|
| **Views** (`Views/`) | WPF `MainWindow`; drag-and-drop wiring. |
| **ViewModels** (`ViewModels/`) | `MainViewModel` — commands, async export loop, progress, validation. |
| **Models** (`Models/`) | `FileItem` (path, status, errors); `ConversionSettings` (simple settings shape). |
| **Services** (`Services/`) | `SolidWorksService` — COM connect, open drawing, PDF/DWG export, close doc; `FileService` — `.slddrw` / folder validation; `DialogService` — WinForms open-file and folder browsers. |
| **Commands** (`Commands/`) | `RelayCommand` for UI commands. |
| **Converters** (`Converters/`) | WPF value converters for visibility and status colors. |

The UI stack is **WPF**; file/folder dialogs use **Windows Forms** for familiar multiselect and folder picker behavior.

---

## Dependencies (NuGet)

From `packages.config`:

- **Xarial.XCad** (0.8.2)  
- **Xarial.XCad.SolidWorks** (0.8.2) — pulls SOLIDWORKS interop assemblies and build targets  
- **Xarial.XCad.Toolkit** (0.8.2)

Restore packages before building; the project imports `Xarial.XCad.SolidWorks.targets` to enforce package presence.

---

## Troubleshooting

| Symptom | Things to check |
|---------|------------------|
| **Could not connect to SOLIDWORKS** | SOLIDWORKS installed; not blocked by antivirus; try starting SOLIDWORKS manually first. |
| **Failed to open document** | File path valid; file not open elsewhere with exclusive lock; drawing not corrupted. |
| **PDF or DWG export failed** | Output folder writable; disk space; SOLIDWORKS export options for your template/sheet. |
| **Build errors about missing targets / packages** | Run NuGet restore; ensure `packages\Xarial.XCad.SolidWorks.0.8.2\` exists. |
| **COM / interop errors after SOLIDWORKS upgrade** | Align interop/NuGet versions with your SOLIDWORKS installation. |

---

## Project structure

```
SolidConvert-Pro/
├── Commands/           # RelayCommand
├── Converters/         # WPF converters
├── Models/             # FileItem, ConversionSettings
├── Properties/         # Assembly info, resources, settings
├── Services/           # SolidWorks, file, dialog services
├── ViewModels/         # MainViewModel, BaseViewModel
├── Views/              # MainWindow (XAML + code-behind)
├── App.xaml(.cs)       # Application entry
├── App.config          # .NET 4.7.2 startup
├── packages.config     # NuGet packages
├── SolidConvert Pro.csproj
└── SolidConvert Pro.sln
```

---

---

## Contributing

Issues and pull requests are welcome. When changing SOLIDWORKS automation, test against the SOLIDWORKS versions you intend to support and document any new custom properties or export assumptions in this README.
