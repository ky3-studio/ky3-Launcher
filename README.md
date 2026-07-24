<p align="center">
  <img src="logo.png" width="120" alt="ky3 launcher logo"/>
</p>

<h1 align="center">ky3 Launcher</h1>

<p align="center">
  <strong>A modern third-party launcher for Genshin Impact, built with WinUI 3</strong>
</p>

<p align="center">
  English | <a href="README.zh-CN.md">简体中文</a>
</p>

<p align="center">
  <img src="https://img.shields.io/github/v/tag/ky3-studio/ky3-Launcher?style=flat&label=version&color=blue" alt="version"/>
  <img src="https://img.shields.io/github/stars/ky3-studio/ky3-Launcher?style=flat&color=gold" alt="stars"/>
  <a href="https://github.com/ky3-studio/ky3-Launcher/actions/workflows/build.yml"><img src="https://github.com/ky3-studio/ky3-Launcher/actions/workflows/build.yml/badge.svg" alt="build"/></a>
  <img src="https://img.shields.io/badge/platform-Windows%2010%2B-0078d4?style=flat&logo=windows" alt="platform"/>
  <img src="https://img.shields.io/badge/.NET-10.0-512bd4?style=flat&logo=dotnet" alt=".NET"/>
  <img src="https://img.shields.io/badge/WinUI-3-7b52ab?style=flat" alt="WinUI 3"/>
  <img src="https://img.shields.io/badge/license-MIT-green?style=flat" alt="license"/>
  <img src="https://img.shields.io/badge/arch-x64-lightgrey?style=flat" alt="arch"/>
  <a href="https://github.com/ky3-studio/ky3-Launcher/releases/latest"><img src="https://img.shields.io/github/v/release/ky3-studio/ky3-Launcher?style=flat&label=download&color=orange" alt="download"/></a>
</p>

---

## Preview

<p align="center">
  <img src="preview.png" width="800" alt="ky3 launcher preview" />
</p>

---

## Localization

![zh-CN](https://img.shields.io/badge/zh--CN-100%25-blue?style=flat&logo=crowdin)
![en-US](https://img.shields.io/badge/en--US-96%25-blue?style=flat&logo=crowdin)
![ja-JP](https://img.shields.io/badge/ja--JP-0%25-red?style=flat&logo=crowdin)
![ko-KR](https://img.shields.io/badge/ko--KR-0%25-red?style=flat&logo=crowdin)
![zh-TW](https://img.shields.io/badge/zh--TW-98%25-blue?style=flat&logo=crowdin)
![ru-RU](https://img.shields.io/badge/ru--RU-0%25-red?style=flat&logo=crowdin)
![de-DE](https://img.shields.io/badge/de--DE-0%25-red?style=flat&logo=crowdin)
![it-IT](https://img.shields.io/badge/it--IT-0%25-red?style=flat&logo=crowdin)
![th-TH](https://img.shields.io/badge/th--TH-0%25-red?style=flat&logo=crowdin)
![vi-VN](https://img.shields.io/badge/vi--VN-0%25-red?style=flat&logo=crowdin)

---

## Features

| Feature | Description |
|------|------|
| Game launching | Custom launch arguments, command-line injection, window mode selection, fast multi-account switching, automatic DLL plugin loading |
| Character planning | Level ascension material calculator, talent upgrade planning, resin cost estimation, daily/weekly boss reward statistics |
| Weapon & character wiki | Browse all characters/weapons with stats, filtering and sorting, constellation & refinement data, recommended artifact builds |
| Quest collection | Archon & Story Quest overview, quest reward statistics, filter by region |
| Achievement tracking | Local achievement state reading powered by [Yae](https://github.com/HolographicHat/Yae), categorized browsing and search |
| Auto check-in | Daily automatic check-in for miHoYo BBS / HoYoLab, batch execution across multiple accounts, result notifications |
| Incremental updates | File-level delta patching (BsDiff), resumable downloads, local verification — no need to re-download the full package |
| Plugin management | Hot loading/unloading of DLL mods, version management, one-click enable/disable |
| Real-Time Notes | Live view of resin / expeditions / Realm Currency / Parametric Transformer, synced per account |
| Wish analysis | Gacha record import, pity counter, drop rate statistics, history visualization |

---

## Tech Stack

| Layer | Technology | Description |
|:---:|------|------|
| Framework | .NET 10 / WinUI 3 | Windows App SDK, modern desktop UI framework |
| Language | C# 14 (preview) | Latest language features, Source Generators |
| Architecture | MVVM | CommunityToolkit.Mvvm, ObservableProperty, WeakReferenceMessenger |
| Database | SQLite | Entity Framework Core ORM, local persistent storage |
| Networking | HttpClient + WebView2 | REST API requests, cookie management, embedded web view |
| Native layer | C++ / Win32 Interop | Native DLL injection, process management, low-level system calls |
| Updater | Rust | Lightweight self-update component with resumable downloads and file-level delta replacement |
| Build | MSBuild + Cargo | Source Generators, incremental compilation, GitHub Actions CI/CD |

---

## Requirements

| Item | Requirement | Notes |
|:---:|------|------|
| OS | Windows 10 2004+ | Build 19041 or later, Windows 11 recommended |
| Architecture | x64 | 64-bit systems only |
| Runtime | VC++ 2015–2022 x64 | [Download](https://aka.ms/vs/17/release/vc_redist.x64.exe) |
| WebView2 | WebView2 Runtime | Any version, built into Windows 11 |

---

## Installation

1. Download the latest version from [Releases](https://github.com/ky3-studio/ky3-Launcher/releases/latest)
2. Extract to any directory
3. Run `launcher.exe`

> First run requires [VC++ 2015–2022 x64](https://aka.ms/vs/17/release/vc_redist.x64.exe) and the [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703), both built into Windows 11.

---

## Building

### Prerequisites

| Dependency | Description |
|------|------|
| <img src="https://img.shields.io/badge/-.NET%2010%20SDK-512bd4?style=flat&logo=dotnet&logoColor=white" alt=""/> | [dotnet.microsoft.com](https://dotnet.microsoft.com/download), preview channel |
| <img src="https://img.shields.io/badge/-Visual%20Studio%202022-5c2d91?style=flat&logo=visualstudio&logoColor=white" alt=""/> | Or build standalone with the `dotnet` CLI |
| <img src="https://img.shields.io/badge/-Windows%20App%20SDK-0078d4?style=flat&logo=windows&logoColor=white" alt=""/> | Select the corresponding workload in VS Installer |
| <img src="https://img.shields.io/badge/-Rust%20Toolchain-000000?style=flat&logo=rust&logoColor=white" alt=""/> | [rustup.rs](https://rustup.rs/), used to compile the updater-rs component |

### Clone the repository

```bash
git clone https://github.com/ky3-studio/ky3-Launcher.git
cd ky3-Launcher
```

### Restore dependencies

```bash
dotnet restore src/launcher/launcher.csproj
```

### Build

```bash
dotnet build src/launcher/launcher.csproj -c Release -p:Platform=x64
```

Build output goes to `src/launcher/bin/Release/`.

### Run tests

```bash
dotnet test tests/ky3launcher.Tests/ky3launcher.Tests.csproj -c Release
```

### Run benchmarks

```bash
dotnet run -c Release --project tests/ky3launcher.Benchmarks -- --filter *
```

---

## Project Structure

```
ky3-Launcher/
├── src/
│   ├── launcher/              ← Main project (WinUI 3)
│   │   ├── Core/              Infrastructure (DI, IO, caching, database, process management)
│   │   ├── Extension/         Extension methods (Span, String, Collection, memory management)
│   │   ├── Model/             Entity models and data structures
│   │   ├── Service/           Business services (planning calculator, wish analysis, check-in, notes)
│   │   ├── ViewModel/         MVVM ViewModel layer
│   │   ├── UI/                UI controls and styles
│   │   ├── Web/               HTTP clients and API wrappers
│   │   ├── Win32/             P/Invoke and native interop
│   │   └── Factory/           UI component factories (dialogs, file pickers, progress bars)
│   └── SourceGeneration/      Source Generators
├── Runner/                    C++ auto-start management (Windows Task Scheduler)
├── tests/
│   ├── ky3launcher.Tests/         Unit tests (341 tests)
│   └── ky3launcher.Benchmarks/    Performance benchmarks
├── module/                    Plugin DLLs
├── Installer/                 Installer scripts (Inno Setup)
└── .github/                   CI/CD, issue templates, Dependabot
```

---

## Related Repositories

| Repository | Description |
|------|------|
| <img src="https://img.shields.io/badge/-plugin--module-181717?style=flat&logo=github&logoColor=white" alt=""/> [ky3-launcher-plugin-module](https://github.com/ky3-git/ky3-launcher-plugin-module) | DLL plugin module providing the plugin SDK and interface definitions |
| <img src="https://img.shields.io/badge/-metadata-181717?style=flat&logo=github&logoColor=white" alt=""/> [ky3-metadata](https://github.com/ky3-git/ky3-metadata) | Game metadata JSON collection (characters / weapons / materials / quests) |
| <img src="https://img.shields.io/badge/-Yae-181717?style=flat&logo=github&logoColor=white" alt=""/> [Yae](https://github.com/HolographicHat/Yae) | Genshin achievement unlock tool, reads achievement state locally |

---

## Contributing

Issues and Pull Requests are welcome to help improve the project.

| Channel | Purpose |
|------|------|
| <img src="https://img.shields.io/badge/-Issue-da3633?style=flat&logo=github&logoColor=white" alt=""/> [Issues](https://github.com/ky3-studio/ky3-Launcher/issues) | Report bugs, suggest features, discuss ideas |
| <img src="https://img.shields.io/badge/-PR-2ea44f?style=flat&logo=github&logoColor=white" alt=""/> [Pull Requests](https://github.com/ky3-studio/ky3-Launcher/pulls) | Fix issues, add features, improve performance |

---

## Acknowledgements

This project takes architectural inspiration from <img src="https://avatars.githubusercontent.com/u/49308723?s=20" width="20" height="20" alt="DGP Studio"/> [Snap Hutao](https://github.com/DGP-Studio/Snap.Hutao). Thanks to [DGP Studio](https://github.com/DGP-Studio) and its community contributors for their open-source work.

---

## License

<img src="https://img.shields.io/badge/license-MIT-green?style=flat&logo=opensourceinitiative&logoColor=white" alt="MIT"/>

This project is open-sourced under the [MIT](LICENSE) license.

---

## Disclaimer

This is a third-party open-source tool with no affiliation with miHoYo / HoYoverse. Users assume all responsibility for any consequences of using this software.
