# Privacy Policy / 隐私政策

**Last Updated / 最后更新：2026-06-1**

---

## English

### Introduction

ky3-Launcher ("the Application") respects your privacy. This Privacy Policy explains what data we collect, how we use it, and your rights regarding that data.

### Data We Collect

The Application collects the following data **solely for crash reporting and quality improvement purposes**:

| Data Type | Description | Purpose |
|-----------|-------------|---------|
| Device ID | An anonymized identifier generated from your machine's registry GUID (hashed, not reversible) | Correlate crash reports from the same device |
| IP Address | Your public IP address at the time of a crash | Geographic error distribution analysis |
| Operating System | Windows version and build number | Identify OS-specific issues |
| Application Version | The version of ky3-Launcher you are running | Track which versions have bugs |
| Crash Data | Exception stack traces, error messages, and breadcrumb logs | Diagnose and fix software bugs |
| Session Information | Anonymous session start/end timestamps | Understand crash frequency |
| Elevation Status | Whether the application is running as administrator | Debug permission-related issues |
| WebView2 Version | Version of the WebView2 runtime installed | Identify browser component issues |
| Device Name | Your computer's machine name (sent to our backend only) | Device identification for account services |

### Data We Do NOT Collect

- Personal identity information (real name, ID number, phone number)
- Game account credentials or passwords
- Game data or save files
- Screenshots or screen recordings
- Keyboard input or clipboard content
- File system contents beyond application configuration
- Browsing history

### How We Use the Data

All collected data is used exclusively to:

1. **Identify and fix software crashes** — Stack traces help us locate bugs
2. **Improve application stability** — Session data helps prioritize fixes
3. **Monitor release quality** — Version-specific crash rates guide development

We do **NOT** use collected data for:
- Advertising or marketing
- User profiling or behavioral analysis
- Sale to third parties

### Third-Party Services

We use the following third-party service for crash reporting:

- **Sentry** (Functional Software, Inc.)
  - Purpose: Error tracking and crash reporting
  - Privacy Policy: https://sentry.io/privacy/
  - Data Processing: https://sentry.io/legal/dpa/

### Data Retention

- Crash reports are retained for **90 days** on Sentry's servers, then automatically deleted.
- No crash data is stored permanently.

### Data Security

- All data transmission uses HTTPS/TLS encryption.
- The anonymized Device ID cannot be reversed to identify you personally.
- Access to crash reporting data is restricted to project maintainers.

### Your Rights

You may:
- **Opt out**: Disable crash reporting by blocking network access to `*.sentry.io` via firewall rules.
- **Request deletion**: Contact us to request deletion of data associated with your Device ID.
- **Inquire**: Ask us what data we have collected about your device.

### Contact

For privacy-related inquiries, please open an issue on our GitHub repository or contact the project maintainer.

### Changes to This Policy

We may update this Privacy Policy from time to time. Changes will be posted in this document with an updated "Last Updated" date.

---

## 中文

### 简介

ky3-Launcher（"本应用程序"）尊重您的隐私。本隐私政策说明我们收集哪些数据、如何使用这些数据以及您的相关权利。

### 我们收集的数据

本应用程序**仅出于崩溃报告和质量改进目的**收集以下数据：

| 数据类型 | 描述 | 用途 |
|---------|------|------|
| 设备 ID | 由您计算机注册表 GUID 生成的匿名标识符（哈希处理，不可逆） | 关联同一设备的崩溃报告 |
| IP 地址 | 崩溃发生时您的公网 IP 地址 | 分析错误的地理分布 |
| 操作系统 | Windows 版本和 Build 号 | 识别特定系统版本的问题 |
| 应用版本 | 您正在运行的 ky3-Launcher 版本 | 追踪哪些版本存在 Bug |
| 崩溃数据 | 异常堆栈跟踪、错误消息和面包屑日志 | 诊断和修复软件 Bug |
| 会话信息 | 匿名会话开始/结束时间戳 | 了解崩溃频率 |
| 权限状态 | 应用程序是否以管理员身份运行 | 调试权限相关问题 |
| WebView2 版本 | 已安装的 WebView2 运行时版本 | 识别浏览器组件问题 |
| 设备名称 | 您计算机的机器名（仅发送至我们的后端） | 账户服务的设备识别 |

### 我们不收集的数据

- 个人身份信息（真实姓名、身份证号、手机号）
- 游戏账号凭据或密码
- 游戏数据或存档文件
- 截图或屏幕录像
- 键盘输入或剪贴板内容
- 应用配置之外的文件系统内容
- 浏览历史

### 数据使用方式

所有收集的数据仅用于：

1. **识别和修复软件崩溃** — 堆栈跟踪帮助我们定位 Bug
2. **提高应用稳定性** — 会话数据帮助确定修复优先级
3. **监控发布质量** — 特定版本的崩溃率指导开发方向

我们**不会**将收集的数据用于：
- 广告或营销
- 用户画像或行为分析
- 向第三方出售

### 第三方服务

我们使用以下第三方服务进行崩溃报告：

- **Sentry**（Functional Software, Inc.）
  - 用途：错误跟踪和崩溃报告
  - 隐私政策：https://sentry.io/privacy/
  - 数据处理协议：https://sentry.io/legal/dpa/

### 数据保留

- 崩溃报告在 Sentry 服务器上保留 **90 天**，之后自动删除。
- 不会永久存储任何崩溃数据。

### 数据安全

- 所有数据传输使用 HTTPS/TLS 加密。
- 匿名设备 ID 无法被逆向还原以识别您的身份。
- 崩溃报告数据的访问仅限项目维护者。

### 您的权利

您可以：
- **退出**：通过防火墙规则阻止对 `*.sentry.io` 的网络访问来禁用崩溃报告。
- **请求删除**：联系我们请求删除与您设备 ID 关联的数据。
- **查询**：询问我们收集了哪些关于您设备的数据。

### 联系方式

如有隐私相关问题，请在我们的 GitHub 仓库提交 Issue 或联系项目维护者。

### 政策变更

我们可能会不时更新本隐私政策。变更将在本文档中发布，并更新"最后更新"日期。
