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
graph TB
    subgraph Solution["kyxsan.sln - 解决方案"]
        direction TB
        subgraph AppLayer["kyxsan - 主应用程序"]
            direction TB
            subgraph UILayer["UI Layer"]
                Shell[Shell / MainWindow]
                Pages[Pages / Views]
                Controls[Custom Controls]
                Windowing[Windowing Management]
                Xaml[XAML Behaviors / Converters]
            end

            subgraph VMLayer["ViewModel Layer - MVVM"]
                MainVM[MainViewModel]
                GameVM[Game ViewModel]
                AchievementVM[Achievement ViewModel]
                GachaVM[GachaLog ViewModel]
                WikiVM[Wiki ViewModel]
                SettingVM[Setting ViewModel]
                SpiralAbyssVM[Spiral Abyss ViewModel]
                DailyNoteVM[DailyNote ViewModel]
                UserVM[User ViewModel]
            end

            subgraph ServiceLayer["Service Layer"]
                direction TB
                subgraph GameService["Game Service"]
                    Launcher[Game Launcher]
                    Account[Account Switcher]
                    Config[Game Configuration]
                    FileSystem[Game FileSystem]
                    Locator[Game Locator]
                    Package[Package Management]
                    Launching[Launch Process]
                end

                subgraph DataService["Data Service"]
                    Achievement[Achievement Service]
                    GachaLog[GachaLog Service]
                    SpiralAbyss[Spiral Abyss]
                    RoleCombat[Role Combat]
                    HardChallenge[Hard Challenge]
                    Inventory[Inventory Service]
                    DailyNote[DailyNote Service]
                end

                subgraph UserService["User Service"]
                    UserAuth[User Authentication]
                    UserCollection[User Collection]
                    UserFingerprint[User Fingerprint]
                    ProfilePicture[Profile Picture]
                end

                subgraph OnlineService["Online Service"]
                    AutoSignIn[Auto Sign-In]
                    Announcement[Announcement]
                    Metadata[Metadata Service]
                    RemoteConfig[Remote Config]
                    Update[Update Service]
                end

                subgraph ToolService["Tool Service"]
                    YaeService[Yae Achievement Unlock]
                    Git[Git Service]
                    ThirdParty[Third-Party Tools]
                    Job[Background Jobs]
                end
            end

            subgraph CoreLayer["Core Layer"]
                direction TB
                subgraph Infrastructure["Infrastructure"]
                    Database[SQLite Database]
                    Caching[Image Cache]
                    Setting[Local Settings]
                    Logging[Logging / Diagnostics]
                    IO[IO / File Operations]
                end

                subgraph Threading["Threading / Async"]
                    AsyncLock[Async Lock / Semaphore]
                    TaskContext[Task Context]
                    RateLimiting[Rate Limiting]
                    DispatcherQueue[Dispatcher Queue]
                end

                subgraph Platform["Platform Abstraction"]
                    DI[Dependency Injection]
                    LifeCycle[App LifeCycle]
                    ExceptionService[Exception Handling]
                    Bootstrap[Bootstrap / Startup]
                end
            end

            subgraph WebLayer["Web / Network Layer"]
                Hoyolab[Hoyolab API Client]
                Enka[Enka Network API]
                KyxsanAPI[kyxsan API Client]
                WebView2[WebView2 Bridge]
                HttpContext[HTTP Context / Headers]
            end

            subgraph InputLayer["Input / Automation"]
                HotKey[HotKey System]
                LowLevel[Low-Level Keyboard Hook]
                VirtualKeys[Virtual Keys Mapping]
            end

            subgraph Win32Layer["Win32 / Native Interop"]
                NativeCore[kyxsanNative Core]
                NativeFS[Native FileSystem]
                NativeHotKey[Native HotKey Actions]
                NativeProcess[Native Process Control]
                NotifyIcon[System Tray NotifyIcon]
                WindowSubclass[Window Subclass]
                DeviceCaps[Device Capabilities]
            end
        end

        subgraph Runner["Runner - C++ Native DLL"]
            AutoStart[auto_start_helper]
        end

        subgraph SourceGen["kyxsan.SourceGeneration - 源代码生成器"]
            AttrGen[Attribute Generator]
            AutomationGen[Automation Code Gen]
            DIGen[DI Registration Gen]
            ModelGen[Model Code Gen]
            XamlGen[XAML Code Gen]
            ResxGen[Resx Localization Gen]
        end

        subgraph Installer["kyxsan.Installer - Inno Setup 安装包"]
            InnoSetup[setup.iss]
            Redist[VC++ Redistributable]
        end
    end

    Shell --> VMLayer
    VMLayer --> ServiceLayer
    ServiceLayer --> CoreLayer
    ServiceLayer --> WebLayer
    InputLayer --> Win32Layer
    Win32Layer --> Runner
    SourceGen -.->|compile-time generation| AppLayer
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
