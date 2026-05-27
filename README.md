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

### 模块总览

```mermaid
graph LR
    A[ky3 Launcher] --> B[UI Layer]
    A --> C[Service Layer]
    A --> D[Core Layer]
    A --> E[Native Layer]

    B --> B1[Pages / Views]
    B --> B2[ViewModels]
    B --> B3[Controls]
    B --> B4[Shell Window]

    C --> C1[Game Launch]
    C --> C2[Achievement]
    C --> C3[Gacha Log]
    C --> C4[User / Auth]
    C --> C5[Yae Integration]
    C --> C6[Auto Sign-In]

    D --> D1[SQLite]
    D --> D2[Image Cache]
    D --> D3[Local Settings]
    D --> D4[Threading]
    D --> D5[IO / FileSystem]

    E --> E1[Runner.dll]
    E --> E2[HotKey Hook]
    E --> E3[Win32 Interop]
```

### 分层架构

```mermaid
graph TD
    subgraph UI["UI Layer — WinUI 3 / XAML"]
        Shell[Shell Window] & Pages[Pages] & Controls[Controls] & Windowing[Windowing]
    end

    subgraph ViewModel["ViewModel — CommunityToolkit.Mvvm"]
        GameVM[Game] & AchievementVM[Achievement] & GachaVM[Gacha] & UserVM[User] & SettingVM[Setting] & WikiVM[Wiki]
    end

    subgraph Services["Service Layer"]
        direction LR
        subgraph Game["Game"]
            Launcher[Launcher] & Account[Account] & Config[Config] & Package[Package]
        end
        subgraph Data["Data"]
            Achievement[Achievement] & GachaLog[GachaLog] & SpiralAbyss[Abyss] & DailyNote[DailyNote]
        end
        subgraph Online["Online"]
            SignIn[Auto Sign-In] & Announce[Announcement] & Update[Update] & Metadata[Metadata]
        end
        subgraph Tools["Tools"]
            Yae[Yae Unlock] & Git[Git] & Jobs[Background Jobs]
        end
    end

    subgraph Core["Core Infrastructure"]
        DB[(SQLite)] & Cache[Image Cache] & Settings[Local Settings] & Threading[Async / Lock] & IO[IO]
    end

    subgraph Web["Web / Network"]
        Hoyolab[Hoyolab API] & Enka[Enka API] & KyxsanAPI[kyxsan API] & WebView2[WebView2]
    end

    subgraph Native["Native — C++ / Win32"]
        Runner[Runner.dll] & HotKey[HotKey Hook] & Win32[Win32 Interop] & Tray[System Tray]
    end

    subgraph SourceGen["Source Generation — Compile Time"]
        DIGen[DI Gen] & ModelGen[Model Gen] & XamlGen[XAML Gen] & ResxGen[Resx Gen]
    end

    UI --> ViewModel --> Services
    Services --> Core
    Services --> Web
    Native --> Runner
    SourceGen -.-> UI
    SourceGen -.-> Services
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
