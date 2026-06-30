<p align="center">
  <img src="logo.png" width="120" alt="ky3 launcher logo"/>
</p>

<h1 align="center">ky3 Launcher</h1>

<p align="center">
  <strong>一款现代化的原神第三方启动器，基于 WinUI 3 构建。</strong>
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

## 预览

<p align="center">
  <img src="preview.png" width="800" alt="ky3 launcher preview" />
</p>

---

## 本地化

![zh-CN](https://img.shields.io/badge/zh--CN-100%25-blue?style=flat&logo=crowdin)
![en-US](https://img.shields.io/badge/en--US-0%25-red?style=flat&logo=crowdin)
![ja-JP](https://img.shields.io/badge/ja--JP-0%25-red?style=flat&logo=crowdin)
![ko-KR](https://img.shields.io/badge/ko--KR-0%25-red?style=flat&logo=crowdin)
![zh-TW](https://img.shields.io/badge/zh--TW-0%25-red?style=flat&logo=crowdin)
![ru-RU](https://img.shields.io/badge/ru--RU-0%25-red?style=flat&logo=crowdin)
![de-DE](https://img.shields.io/badge/de--DE-0%25-red?style=flat&logo=crowdin)
![it-IT](https://img.shields.io/badge/it--IT-0%25-red?style=flat&logo=crowdin)
![th-TH](https://img.shields.io/badge/th--TH-0%25-red?style=flat&logo=crowdin)
![vi-VN](https://img.shields.io/badge/vi--VN-0%25-red?style=flat&logo=crowdin)

---

## 功能特性

| 功能 | 说明 |
|------|------|
| 游戏启动 | 自定义启动参数、命令行注入、窗口模式选择、多账号快速切换、DLL 插件自动加载 |
| 角色养成 | 等级突破材料计算、天赋升级规划、树脂消耗预估、每日/周本收益统计 |
| 武器 & 角色百科 | 全角色 / 武器属性浏览、筛选排序、命之座 & 精炼数据、圣遗物推荐搭配 |
| 任务合集 | 魔神任务 & 传说任务总览、任务奖励统计、按区域筛选 |
| 成就追踪 | 基于 [Yae](https://github.com/HolographicHat/Yae) 的本地成就状态读取、分类浏览与搜索 |
| 自动签到 | 米游社 / HoYoLab 每日自动签到、多账号批量执行、签到结果通知 |
| 增量更新 | 文件级增量补丁（BsDiff）、断点续传、本地校验、无需重新下载完整安装包 |
| 插件管理 | DLL 模组热加载 / 卸载、版本管理、一键启用 / 禁用 |
| 实时便笺 | 树脂 / 委托 / 洞天宝钱 / 参量质变仪实时查看、数据与账号联动 |
| 祈愿分析 | 抽卡记录导入、保底计数、出货概率统计、历史记录可视化 |

---

## 技术栈

| 层 | 技术 | 说明 |
|:---:|------|------|
| 框架 | .NET 10 / WinUI 3 | Windows App SDK，现代桌面 UI 框架 |
| 语言 | C# 14 (preview) | 最新语言特性、Source Generators 代码生成 |
| 架构 | MVVM | CommunityToolkit.Mvvm、ObservableProperty、WeakReferenceMessenger |
| 数据库 | SQLite | Entity Framework Core ORM、本地持久化存储 |
| 网络 | HttpClient + WebView2 | REST API 请求、Cookie 管理、嵌入式 Web 视图 |
| 原生层 | C++ / Win32 Interop | Native DLL 注入、进程管理、底层系统调用 |
| 更新器 | Rust | 轻量级自更新组件，断点续传、文件级增量替换 |
| 构建 | MSBuild + Cargo | Source Generators、增量编译、GitHub Actions CI/CD |

---

## 环境要求

| 项 | 要求 | 备注 |
|:---:|------|------|
| OS | Windows 10 2004+ | Build 19041 及以上，推荐 Windows 11 |
| 架构 | x64 | 仅支持 64 位系统 |
| 运行库 | VC++ 2015–2022 x64 | [下载地址](https://aka.ms/vs/17/release/vc_redist.x64.exe) |
| WebView2 | WebView2 Runtime | 任意版本，Windows 11 已内置 |

---

## 安装

1. 前往 [Releases](https://github.com/ky3-studio/ky3-Launcher/releases/latest) 下载最新版本
2. 解压到任意目录
3. 运行 `launcher.exe`

> 首次运行需要 [VC++ 2015–2022 x64](https://aka.ms/vs/17/release/vc_redist.x64.exe) 和 [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703)，Windows 11 已内置。

---

## 构建

### 前置条件

| 依赖 | 说明 |
|------|------|
| <img src="https://img.shields.io/badge/-.NET%2010%20SDK-512bd4?style=flat&logo=dotnet&logoColor=white" alt=""/> | [dotnet.microsoft.com](https://dotnet.microsoft.com/download)，preview 版本 |
| <img src="https://img.shields.io/badge/-Visual%20Studio%202022-5c2d91?style=flat&logo=visualstudio&logoColor=white" alt=""/> | 或使用 `dotnet` CLI 单独构建 |
| <img src="https://img.shields.io/badge/-Windows%20App%20SDK-0078d4?style=flat&logo=windows&logoColor=white" alt=""/> | VS Installer 中勾选对应 Workload |
| <img src="https://img.shields.io/badge/-Rust%20Toolchain-000000?style=flat&logo=rust&logoColor=white" alt=""/> | [rustup.rs](https://rustup.rs/)，用于编译 updater-rs 更新器组件 |

### 克隆仓库

```bash
git clone https://github.com/ky3-studio/ky3-Launcher.git
cd ky3-Launcher
```

### 还原依赖

```bash
dotnet restore src/launcher/launcher.csproj
```

### 编译

```bash
dotnet build src/launcher/launcher.csproj -c Release -p:Platform=x64
```

构建产物输出到 `bin/Release/` 目录。

### 运行测试

```bash
dotnet test tests/ky3launcher.Tests/ky3launcher.Tests.csproj -c Release
```

### 运行性能基准

```bash
dotnet run -c Release --project tests/ky3launcher.Benchmarks -- --filter *
```

---

## 项目结构

```
ky3-Launcher/
├── src/
│   ├── launcher/              ← 主项目 (WinUI 3)
│   │   ├── Core/              基础设施 (DI、IO、缓存、数据库、进程管理)
│   │   ├── Extension/         扩展方法 (Span、String、Collection、内存管理)
│   │   ├── Model/             实体模型与数据结构
│   │   ├── Service/           业务服务 (养成计算、祈愿分析、签到、便笺)
│   │   ├── ViewModel/         MVVM ViewModel 层
│   │   ├── UI/                界面控件与样式
│   │   ├── Web/               HTTP 客户端与 API 封装
│   │   ├── Win32/             P/Invoke 与原生交互
│   │   └── Factory/           UI 组件工厂 (对话框、文件选择器、进度条)
│   └── SourceGeneration/      Source Generators
├── Runner/                    C++ 自动启动管理 (Windows Task Scheduler)
├── tests/
│   ├── ky3launcher.Tests/         单元测试 (247 tests)
│   └── ky3launcher.Benchmarks/    性能基准测试
├── module/                    插件 DLL
├── Installer/                 安装包脚本 (Inno Setup)
└── .github/                   CI/CD、Issue 模板、Dependabot
```

---

## 路线图

| 状态 | 功能 |
|:---:|------|
| ✅ | 游戏启动、多账号切换、DLL 插件加载 |
| ✅ | 角色/武器养成计算器 |
| ✅ | 实时便笺、自动签到、祈愿分析 |
| ✅ | 文件级增量更新 |
| 🚧 | 多语言国际化 (en-US / ja-JP / ko-KR) |
| 💡 | 背包物品解析 |
| 💡 | 圣遗物评分与强化建议 |

---

## 常见问题

<details>
<summary><b>启动时白屏 / 闪退</b></summary>

1. 确认已安装 [VC++ 2015–2022 x64](https://aka.ms/vs/17/release/vc_redist.x64.exe)
2. 确认已安装 [WebView2 Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703)
3. 尝试以管理员身份运行
4. 检查是否被杀毒软件拦截
</details>

<details>
<summary><b>网络请求失败 / 签到失败</b></summary>

1. 可能返回5003 风控 稍后再试
2. 确认 Cookie 未过期，尝试重新登录米游社账号
</details>

<details>
<summary><b>插件加载失败 / DLL 报错</b></summary>

1. 确保 DLL 架构为 x64，不支持 x86
2. 检查 DLL 是否放在 exe 同目录下
3. 查看日志文件定位具体错误原因
</details>

<details>
<summary><b>自动更新失败</b></summary>

1. 检查网络连接是否正常
2. 确认程序目录有写入权限
3. 手动下载 [Latest Release](https://github.com/ky3-studio/ky3-Launcher/releases/latest) 覆盖安装
</details>

<details>
<summary><b>树脂 / 便笺数据不更新</b></summary>

1. 检查账号是否已绑定并登录
2. 米游社 API 偶尔延迟，稍后重试
3. 5003 风控问题
</details>

---

## 相关仓库

| 仓库 | 说明 |
|------|------|
| <img src="https://img.shields.io/badge/-plugin--module-181717?style=flat&logo=github&logoColor=white" alt=""/> [ky3-launcher-plugin-module](https://github.com/ky3-git/ky3-launcher-plugin-module) | DLL 插件模块，提供插件 SDK 与接口定义 |
| <img src="https://img.shields.io/badge/-metadata-181717?style=flat&logo=github&logoColor=white" alt=""/> [ky3-metadata](https://github.com/ky3-git/ky3-metadata) | 游戏元数据 JSON 集合（角色 / 武器 / 材料 / 任务） |
| <img src="https://img.shields.io/badge/-Yae-181717?style=flat&logo=github&logoColor=white" alt=""/> [Yae](https://github.com/HolographicHat/Yae) | 原神成就解锁工具，本地读取成就状态 |

---

## 贡献

欢迎提交 Issue 和 Pull Request，帮助改进项目。

| 渠道 | 用途 |
|------|------|
| <img src="https://img.shields.io/badge/-Issue-da3633?style=flat&logo=github&logoColor=white" alt=""/> [Issues](https://github.com/ky3-studio/ky3-Launcher/issues) | 反馈 Bug、提出功能建议、讨论方案 |
| <img src="https://img.shields.io/badge/-PR-2ea44f?style=flat&logo=github&logoColor=white" alt=""/> [Pull Requests](https://github.com/ky3-studio/ky3-Launcher/pulls) | 修复问题、新增功能、优化性能 |

---

## 致谢

本项目参考了 <img src="https://avatars.githubusercontent.com/u/49308723?s=20" width="20" height="20" alt="DGP Studio"/> [Snap Hutao](https://github.com/DGP-Studio/Snap.Hutao) 的架构设计，感谢 [DGP Studio](https://github.com/DGP-Studio) 及其社区贡献者的开源工作。

---

## License

<img src="https://img.shields.io/badge/license-MIT-green?style=flat&logo=opensourceinitiative&logoColor=white" alt="MIT"/>

本项目基于 [MIT](LICENSE) 协议开源。

---

## Star History

<a href="https://star-history.com/#ky3-studio/ky3-Launcher&Date">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://api.star-history.com/svg?repos=ky3-studio/ky3-Launcher&type=Date&theme=dark" />
    <source media="(prefers-color-scheme: light)" srcset="https://api.star-history.com/svg?repos=ky3-studio/ky3-Launcher&type=Date" />
    <img alt="Star History Chart" src="https://api.star-history.com/svg?repos=ky3-studio/ky3-Launcher&type=Date" />
  </picture>
</a>

---

## 免责声明

本项目为第三方开源工具，与 miHoYo / HoYoverse 无任何关联。使用本软件产生的一切后果由用户自行承担。
