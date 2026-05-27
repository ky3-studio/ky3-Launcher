<p align="center">
  <img src="logo.png" width="120" alt="ky3 launcher logo"/>
</p>

<h1 align="center">ky3 launcher</h1>

<p align="center">
  <img src="https://img.shields.io/badge/version-6.6.1-blue?style=flat-square" alt="version"/>
  <img src="https://img.shields.io/badge/platform-Windows%2010%2B-0078d4?style=flat-square&logo=windows" alt="platform"/>
  <img src="https://img.shields.io/badge/.NET-10.0-512bd4?style=flat-square&logo=dotnet" alt=".NET"/>
  <img src="https://img.shields.io/badge/WinUI-3-7b52ab?style=flat-square" alt="WinUI 3"/>
  <img src="https://img.shields.io/badge/license-MIT-green?style=flat-square" alt="license"/>
  <img src="https://img.shields.io/badge/arch-x64-lightgrey?style=flat-square" alt="arch"/>
</p>

<p align="center">原神（Genshin Impact）第三方启动器，基于 WinUI 3 构建。</p>

## 架构

```mermaid
graph LR
    A[ky3 Launcher] --> B[UI]
    A --> C[Services]
    A --> D[Core]
    A --> E[Native]

    B --> B1[Pages] & B2[ViewModels] & B3[Controls]
    C --> C1[Game] & C2[Achievement] & C3[Gacha] & C4[User] & C5[Yae]
    D --> D1[SQLite] & D2[Cache] & D3[Settings] & D4[Threading]
    E --> E1[Runner.dll] & E2[HotKey] & E3[Win32]
```

```mermaid
graph TD
    subgraph UI["UI — WinUI 3"]
        Pages & ViewModels & Controls
    end

    subgraph Services["Services"]
        Game & Achievement & Gacha & User & Yae & SignIn[Auto Sign-In]
    end

    subgraph Core["Core"]
        DB[(SQLite)] & Cache & Settings & IO
    end

    subgraph Native["Native — C++"]
        Runner & HotKey & Win32
    end

    UI --> Services --> Core
    Services --> Native
```

## 功能

- 游戏启动与插件管理
- 包含部分 Wiki 百科
- 成就追踪
- 自动签到
- DLL 模组加载管理
- 多语言本地化支持（zh-CN）

## 技术栈

- .NET 10 / WinUI 3（Windows App SDK）
- C# 13（preview）
- MVVM Toolkit
- SQLite（本地数据库）
- WebView2

## 环境要求

| 项 | 要求 |
|---|---|
| 操作系统 | Windows 10 2004（19041）及以上 |
| 架构 | x64 |
| VC++ 运行库 | 2015–2022 x64 |
| WebView2 运行时 | 任意版本 |

## 相关仓库

| 仓库 | 说明 |
|---|---|
| [ky3-launcher-plugin-module](https://github.com/ky3-git/ky3-launcher-plugin-module) | 本项目使用的 DLL 插件模块 |
| [il2cpp-dump](https://github.com/ky3-git/il2cpp-dump) | IL2CPP 元数据 Dump 工具 |
| [ky3-metadata](https://github.com/ky3-git/ky3-metadata) | 游戏元数据 JSON 集合 |
| [Yae](https://github.com/HolographicHat/Yae) | 原神成就解锁工具，实现非常出色，本项目成就追踪功能的参考之一 |

## 贡献

欢迎提交 Issue 和 Pull Request。

- [**Issue**](https://github.com/ky3-git/ky3-Launcher/issues)：反馈 Bug、提出功能建议
- [**Pull Request**](https://github.com/ky3-git/ky3-Launcher/pulls)：修复问题或新增功能，请确保代码可正常编译

## 致谢

本项目基于胡桃启动器的架构设计思路衍生开发，感谢 DGP Studio 和其他贡献成员的开源工作。

## License

[MIT](LICENSE)
