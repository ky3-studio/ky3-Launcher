# ky3 Launcher Privacy Policy

**Last updated: July 2026**
**Applicable version: 6.7.2.0 and later**
**Publisher: KY3 STUDIO**

ky3 Launcher (the "Software") is an open-source (MIT License) third-party launcher for Genshin Impact. We take your privacy seriously. This policy describes—based on the **Software's actual code behavior**—what data the Software collects, stores, uses, and transmits, and where that data goes.

> In short: your account credentials and game data are **stored only on your local device**, and are sent only to **miHoYo / HoYoLab official servers** when you use the relevant features. The Software reports crash/exception information to Sentry, and sends anonymous heartbeat statistics and feedback you actively submit. The Software contains **no** advertising SDK and does **not** collect or sell personal profiling data for commercial analytics.

---

## 1. Data We Collect

### 1.1 Crash and Exception Diagnostics (via Sentry)

The Software integrates Sentry (`Sentry.Extensions.Logging`) for stability diagnostics. When an error occurs (log level Error and above), the following information is reported:

- **Exception details**: exception type, stack trace, error codes (HRESULT / HTTP / Socket / Win32 error codes).
- **Breadcrumbs**: log records at Information level and above generated during app execution, which may include operation steps, partial file paths, or request URLs (no account credentials).
- **Runtime environment**: Windows version and build number, WebView2 version, and whether the process runs with administrator privileges.
- **Device identifier (User.Id)**: see "Device Identifiers" below.
- **IP address**: on startup the Software queries your public IP and attaches it to Sentry events for issue localization and deduplication.

Sentry data is sent to Sentry's official service (US region, `*.ingest.us.sentry.io`). We have configured certain third-party CDN (e.g., `cnb.cool`, `seria.moe`) network errors to be silently handled at the application layer and **not reported**. The Software does **not** enable Sentry's "Send Default PII" (default personal information collection).

### 1.2 Device Identifiers

- **DeviceId**: generated as an MD5 hash of `your Windows username + this machine's MachineGuid`, used as an anonymous device identifier. It is used as the Sentry user identifier and passed via the following request headers when making requests to servers:
  - `x-Launcher-device-id`: the DeviceId above
  - `x-Launcher-device-os`: Windows version number
  - `x-Launcher-device-name`: this machine's computer name (non-ASCII characters escaped)
- **HoYoLab device identifiers**: to access miHoYo/HoYoLab APIs, a set of device identifiers (e.g., `x-rpc-device_id`) is randomly generated within the current session and sent to miHoYo official servers with requests.

### 1.3 Account and Login Credentials

When you log in to a miHoYo Community / HoYoLab account within the Software, the following authentication information is processed and stored locally:

- `LToken`, `SToken`, `CookieToken` (login tokens / cookies)
- Account IDs (`aid` / `mid`), the linked game UID, and device fingerprint

**Storage location and method:**

- All stored in the local SQLite database (`%LocalAppData%\ky3 Launcher\Data\Userdata.db`), in the `users` table.
- **Login tokens (`LToken` / `SToken` / `CookieToken`) are encrypted with Windows DPAPI before storage** (`ProtectedData`, `CurrentUser` scope, Base64-encoded), and can only be decrypted by **the same Windows user account on the same device**.
- Account IDs (`aid` / `mid`), game UID, and device fingerprint are stored in plaintext in the same database.

> Note: general app settings and preferences are kept in a separate settings store (the packaged build uses the Windows app data container; the unpackaged build uses `%LocalAppData%\ky3 Launcher\Settings\LocalSettings.json`, plaintext JSON). This settings store does **not** contain the login tokens above.

**Purpose and flow**: these credentials are used **only** to access **miHoYo / HoYoLab official APIs** on your behalf (e.g., auto check-in, reading real-time notes / Spiral Abyss / character data, wish records), and are **never** uploaded to the Software's servers or any third party.

### 1.4 Heartbeat and Statistics

The Software sends a **heartbeat request** for online status / activity statistics, carrying three items—**device identifier (DeviceId), launcher version, and operating system version string**—sent once on the first launch each day or per version.

### 1.5 Feedback You Actively Submit

When you submit feedback within the Software, the **feedback content** you write and any **images** you choose to upload are sent to the Software's server for issue handling and replies. This is an action you actively trigger.

### 1.6 Other Locally Stored Data (Not Uploaded)

The following data is stored only on your local device for the Software's functionality and is not proactively uploaded:

- App settings and preferences (theme, language, launch parameters, directory configuration, etc.)
- Game paths, plugin enable states
- Cached game data: achievements, daily notes, Spiral Abyss / Imaginarium Theater, character combat records, wish records, etc. (SQLite)
- Image cache, game screenshots

---

## 2. Servers Data Is Sent To

| Category | Server | Purpose |
|------|--------|------|
| miHoYo Official (CN) | `*.mihoyo.com` (passport-api, api-takumi, bbs-api, hyp-api, hk4e-*, etc.) | Account login, check-in, game data, announcements, wishes, downloads |
| HoYoverse Official (Global) | `*.hoyoverse.com`, `*.hoyolab.com` | Same as above (overseas version) |
| Software Server | — | Announcements, heartbeat statistics, feedback, public IP lookup, metadata/incremental patches, static resources |
| Crash Diagnostics | Sentry (`*.ingest.us.sentry.io`, US region) | Exception and crash reporting |
| Character Data | `enka.network` | Query character/equipment showcase data by game UID |
| CAPTCHA | `static.geetest.com` | Human verification for login/check-in |
| Updates & Repository | `github.com/ky3-git/ky3-Launcher` | Version updates, Issue/repository access, proxy connectivity checks |
| Other Third Parties | Mirror/CDN domains delivered by remote configuration (e.g., `cnb.cool`, `seria.moe`) | Static resource mirrors and other non-core services |

---

## 3. What We Do Not Do

- We do **not** collect personal profiling data for advertising or commercial analytics.
- We do **not** integrate third-party commercial analytics SDKs (e.g., Google Analytics, App Insights).
- We do **not** upload your account credentials or game data to the Software's servers, nor sell them to third parties.
- We do **not** covertly collect information unrelated to the Software's functionality in the background.

---

## 4. Data Security and Your Control

- Account credentials and game data are stored on your local device. You can **log out / delete your account** within the Software at any time, or delete the data directory directly to clear local data.
- You can **uninstall the Software** at the system level to stop all data processing.
- Crash diagnostics (Sentry) are used to improve stability; if you do not wish to report, you may block the relevant domains at the network level, or choose not to use the Software.
- Login tokens are encrypted with Windows DPAPI and can only be decrypted under the same Windows user on the same device; do not copy the data directory to an untrusted environment, and protect your Windows account.

---

## 5. Third-Party Services

In the course of providing functionality, the Software interacts with third-party services including miHoYo / HoYoLab, Sentry, GEETEST, Enka.Network, and GitHub. Each service's handling of the data it receives is governed by its own privacy policy and is outside the scope of this policy.

---

## 6. Disclaimer

The Software is an **unofficial** third-party tool and has no affiliation or partnership with Shanghai miHoYo or HoYoverse. Genshin Impact, HoYoLab, miHoYo Community, and related trademarks belong to their respective owners. You assume all risks arising from the use of the Software.

---

## 7. Policy Changes

This policy may be adjusted as the Software's functionality is updated; the updated version will be provided together with the Software release. Significant changes will be noted in the release notes.

---

## 8. Contact Us

If you have any questions about this Privacy Policy, you can reach us via the Issues in the project repository:
`https://github.com/ky3-git/ky3-Launcher`
