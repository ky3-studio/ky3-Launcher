<p align="center">
  <img src="logo.png" width="120" alt="ky3 launcher logo"/>
</p>

<h1 align="center">ky3 Launcher</h1>

<p align="center">
  <strong>一款现代化的原神第三方启动器，基于 WinUI 3 构建</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/version-6.6.3-blue?style=flat-square" alt="version"/>
  <img src="https://img.shields.io/badge/platform-Windows%2010%2B-0078d4?style=flat-square&logo=windows" alt="platform"/>
  <img src="https://img.shields.io/badge/.NET-10.0-512bd4?style=flat-square&logo=dotnet" alt=".NET"/>
  <img src="https://img.shields.io/badge/WinUI-3-7b52ab?style=flat-square" alt="WinUI 3"/>
  <img src="https://img.shields.io/badge/license-MIT-green?style=flat-square" alt="license"/>
  <img src="https://img.shields.io/badge/arch-x64-lightgrey?style=flat-square" alt="arch"/>
</p>

---

## 预览

<p align="center">
  <img src="preview.png" width="800" alt="ky3 launcher preview" />
</p>

---

## 功能特性

| 功能 | 说明 |
|------|------|
| 游戏启动 | 支持自定义启动参数、多账号切换、插件注入 |
| 角色养成 | 材料计算、等级规划、树脂预估 |
| 武器 & 角色百科 | 完整的角色/武器数据浏览与筛选 |
| 任务合集 | 魔神任务总览、奖励统计、区域筛选 |
| 成就追踪 | 基于 Yae 的成就解锁状态读取 |
| 自动签到 | 支持 HoYoLab / 米游社每日自动签到 |
| 增量更新 | 文件级增量补丁，无需重新下载安装包 |
| 插件管理 | DLL 模组热加载与管理 |

---

## 技术栈

| 层 | 技术 |
|---|---|
| 框架 | .NET 10 / WinUI 3 (Windows App SDK) |
| 语言 | C# 13 (preview) |
| 架构 | MVVM (CommunityToolkit.Mvvm) |
| 数据库 | SQLite + Entity Framework Core |
| 网络 | HttpClient + WebView2 |
| 原生层 | C++ Win32 Interop / Native DLL |
| 构建 | MSBuild + Source Generators |

---

## 环境要求

| 项 | 要求 |
|---|---|
| 操作系统 | Windows 10 2004 (19041) 及以上 |
| 架构 | x64 |
| VC++ 运行库 | 2015–2022 x64 |
| WebView2 运行时 | 任意版本 |

---

## 构建

### 前置条件

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (preview)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) 或 `dotnet` CLI
- Windows App SDK workload

### 克隆仓库

```bash
git clone https://github.com/ky3-git/ky3-Launcher.git
cd ky3-Launcher
```

### 还原依赖

```bash
dotnet restore src/launcher/launcher.csproj
```

### 编译

```bash
dotnet build src/launcher/launcher.csproj -c Release
```

构建产物输出到 `bin/Release/` 目录。

---

## 相关仓库

| 仓库 | 说明 |
|------|------|
| [ky3-launcher-plugin-module](https://github.com/ky3-git/ky3-launcher-plugin-module) | DLL 插件模块 |
| [ky3-metadata](https://github.com/ky3-git/ky3-metadata) | 游戏元数据 JSON 集合 |
| [Yae](https://github.com/HolographicHat/Yae) | 原神成就解锁工具 |

---

## 贡献

欢迎提交 Issue 和 Pull Request。

- [**Issue**](https://github.com/ky3-git/ky3-Launcher/issues) — 反馈 Bug、提出功能建议
- [**Pull Request**](https://github.com/ky3-git/ky3-Launcher/pulls) — 修复问题或新增功能

---

## 致谢

本项目参考了 [Snap Hutao](https://github.com/DGP-Studio/Snap.Hutao) 的架构设计，感谢 DGP Studio 及其社区贡献者的开源工作。

---

## License

[MIT](LICENSE)
