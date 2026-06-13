#define AppName       "ky3 Launcher"
#define AppVersion    "6.6.3"
#define AppPublisher  "kyxsan Detail Development Team"
#define AppExe        "ky3launcher.exe"
#define AppId         "{{8F3A1C2E-7B4D-4E5A-9F6B-2A1D3C4E5F60}"
#define SrcDir        "D:\kyxsan-launcher\bin\Release"
#define IconFile      "D:\kyxsan-launcher\ky3 launcher\src\launcher\Assets\Logo.ico"
#define FontName      "Microsoft YaHei UI"
#define OutDir        "D:\kyxsan-launcher\installer"

[Setup]
AppId={#AppId}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppCopyright=Copyright (C) 2026 {#AppPublisher}
VersionInfoVersion={#AppVersion}.0
VersionInfoProductVersion={#AppVersion}.0
VersionInfoDescription={#AppName} Installer

DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
DisableDirPage=no
DisableReadyPage=no
ShowLanguageDialog=no

ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
MinVersion=10.0.19041

OutputDir={#OutDir}
OutputBaseFilename=ky3launcher-setup-{#AppVersion}
Compression=lzma2/ultra64
SolidCompression=yes
LZMAUseSeparateProcess=yes

WizardStyle=modern
WizardSizePercent=110
WizardImageFile=kyxsan.bmp
WizardSmallImageFile=kyxsan-small.bmp
SetupIconFile={#IconFile}
UninstallDisplayIcon={app}\{#AppExe}
UninstallDisplayName={#AppName}

CloseApplications=force
RestartApplications=no
AllowNoIcons=yes

[Languages]
Name: "chs"; MessagesFile: "compiler:ChineseSimplified.isl"

[Tasks]
Name: "desktopicon";   Description: "创建桌面快捷方式";                         GroupDescription: "附加快捷方式:"
Name: "addtopath";     Description: "添加到 PATH (重启后生效)";                 GroupDescription: "其他:";          Flags: unchecked
Name: "fileassoc";     Description: "将 ky3 Launcher 注册为 .ky3 文件的默认打开方式"; GroupDescription: "其他:";          Flags: unchecked

[Files]
Source: "{#SrcDir}\{#AppExe}";  DestDir: "{app}"; Flags: ignoreversion
Source: "{#SrcDir}\*";          DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb,*.lib,*.exp,*.xml,BuildHost-*,obj\*,kyxsan.SourceGeneration.*"
Source: "redist\vc_redist.x64.exe";              DestDir: "{tmp}"; Flags: deleteafterinstall; Check: NeedVCRedist
Source: "redist\MicrosoftEdgeWebView2Setup.exe";  DestDir: "{tmp}"; Flags: deleteafterinstall; Check: NeedWebView2

[Icons]
Name: "{group}\{#AppName}";          Filename: "{app}\{#AppExe}"; IconFilename: "{app}\{#AppExe}"; WorkingDir: "{app}"
Name: "{group}\卸载 {#AppName}";     Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppName}";    Filename: "{app}\{#AppExe}"; IconFilename: "{app}\{#AppExe}"; WorkingDir: "{app}"; Tasks: desktopicon

[Registry]
Root: HKLM; Subkey: "SYSTEM\CurrentControlSet\Control\Session Manager\Environment"; \
    ValueType: expandsz; ValueName: "Path"; ValueData: "{olddata};{app}"; \
    Check: NeedsAddPath('{app}'); Tasks: addtopath

Root: HKCR; Subkey: ".ky3";                              ValueType: string; ValueName: ""; ValueData: "ky3Launcher.Profile"; Flags: uninsdeletevalue; Tasks: fileassoc
Root: HKCR; Subkey: "ky3Launcher.Profile";               ValueType: string; ValueName: ""; ValueData: "ky3 Launcher Profile";  Flags: uninsdeletekey;   Tasks: fileassoc
Root: HKCR; Subkey: "ky3Launcher.Profile\DefaultIcon";   ValueType: string; ValueName: ""; ValueData: "{app}\{#AppExe},0";                              Tasks: fileassoc
Root: HKCR; Subkey: "ky3Launcher.Profile\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#AppExe}"" ""%1""";                Tasks: fileassoc

[Run]
Filename: "{tmp}\vc_redist.x64.exe";              Parameters: "/install /quiet /norestart"; StatusMsg: "正在安装 Visual C++ 运行库...";       Check: NeedVCRedist;       Flags: waituntilterminated
Filename: "{tmp}\MicrosoftEdgeWebView2Setup.exe";  Parameters: "/silent /install";              StatusMsg: "正在安装 WebView2 运行时...";         Check: NeedWebView2;       Flags: waituntilterminated
Filename: "{app}\{#AppExe}"; Description: "立即启动 {#AppName}"; Flags: nowait postinstall skipifsilent
Filename: "{app}\{#AppExe}"; Flags: nowait runasoriginaluser; Check: WizardSilent

[UninstallRun]

[UninstallDelete]
Type: filesandordirs; Name: "{app}\Resources\Metadata"
Type: filesandordirs; Name: "{app}\Cache"
Type: filesandordirs; Name: "{app}\Logs"
Type: dirifempty;     Name: "{app}"

[Code]

var
  GNeedVCRedist: Boolean;
  GNeedWebView2: Boolean;
  GDesktopIconExists: Boolean;

function IsVCRedistInstalled: Boolean;
var
  Installed: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM,
    'SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64',
    'Installed', Installed) and (Installed = 1);
end;

function IsWebView2Installed: Boolean;
var
  Ver: string;
begin
  Result := RegQueryStringValue(HKLM,
    'SOFTWARE\WOW6432Node\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}',
    'pv', Ver) and (Ver <> '') and (Ver <> '0.0.0.0');
  if not Result then
    Result := RegQueryStringValue(HKCU,
      'Software\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}',
      'pv', Ver) and (Ver <> '') and (Ver <> '0.0.0.0');
end;

function NeedVCRedist: Boolean;
begin
  Result := GNeedVCRedist;
end;

function NeedWebView2: Boolean;
begin
  Result := GNeedWebView2;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  UninstStr: string;
  UninstKey: string;
  ResultCode: Integer;
  DesktopPath: string;
  ShortcutPath: string;
begin
  Result := '';
  DesktopPath := ExpandConstant('{autodesktop}');
  ShortcutPath := AddBackslash(DesktopPath) + ExpandConstant('{#AppName}') + '.lnk';
  GDesktopIconExists := FileExists(ShortcutPath);
  if GDesktopIconExists and IsTaskSelected('desktopicon') = False then
    WizardSelectTasks('desktopicon');
  UninstKey := 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\' +
               ExpandConstant('{#SetupSetting("AppId")}') + '_is1';
  if not RegQueryStringValue(HKLM, UninstKey, 'UninstallString', UninstStr) then
    RegQueryStringValue(HKCU, UninstKey, 'UninstallString', UninstStr);
  if UninstStr <> '' then
  begin
    UninstStr := RemoveQuotes(UninstStr);
    Exec(UninstStr, '/VERYSILENT /NORESTART /SUPPRESSMSGBOXES', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;

function InitializeSetup: Boolean;
var
  Msg: string;
begin
  Result := True;
  GNeedVCRedist := not IsVCRedistInstalled;
  GNeedWebView2 := not IsWebView2Installed;

  Msg := '';
  if GNeedVCRedist then
    Msg := Msg + '  · Visual C++ 运行库 2015-2022 x64' + #13#10;
  if GNeedWebView2 then
    Msg := Msg + '  · Microsoft Edge WebView2 运行时' + #13#10;

  if Msg <> '' then
  begin
    if MsgBox(
      '检测到以下运行时依赖未安装：' + #13#10 + #13#10 +
      Msg + #13#10 +
      '安装器将自动静默安装以上依赖，是否继续？',
      mbConfirmation, MB_YESNO) = IDNO then
      Result := False;
  end;
end;

procedure ApplyCustomFontToControl(C: TControl);
var
  I: Integer;
  F: string;
begin
  F := '{#FontName}';
  if C is TLabel             then TLabel(C).Font.Name := F
  else if C is TNewStaticText then TNewStaticText(C).Font.Name := F
  else if C is TNewCheckListBox then TNewCheckListBox(C).Font.Name := F
  else if C is TNewListBox    then TNewListBox(C).Font.Name := F
  else if C is TNewMemo       then TNewMemo(C).Font.Name := F
  else if C is TNewEdit       then TNewEdit(C).Font.Name := F
  else if C is TNewComboBox   then TNewComboBox(C).Font.Name := F
  else if C is TNewCheckBox   then TNewCheckBox(C).Font.Name := F
  else if C is TNewRadioButton then TNewRadioButton(C).Font.Name := F
  else if C is TNewButton     then TNewButton(C).Font.Name := F
  else if C is TButton        then TButton(C).Font.Name := F
  else if C is TNewProgressBar then  
  else if C is TForm          then TForm(C).Font.Name := F;
  if C is TWinControl then
    for I := 0 to TWinControl(C).ControlCount - 1 do
      ApplyCustomFontToControl(TWinControl(C).Controls[I]);
end;

procedure InitializeWizard;
begin
  ApplyCustomFontToControl(WizardForm);
end;

procedure InitializeUninstallProgressForm;
begin
  ApplyCustomFontToControl(UninstallProgressForm);
end;

function NeedsAddPath(Param: string): Boolean;
var
  OrigPath: string;
begin
  if not RegQueryStringValue(HKEY_LOCAL_MACHINE,
       'SYSTEM\CurrentControlSet\Control\Session Manager\Environment',
       'Path', OrigPath) then
  begin
    Result := True;
    exit;
  end;
  Result := Pos(';' + Uppercase(ExpandConstant(Param)) + ';', ';' + Uppercase(OrigPath) + ';') = 0;
end;

procedure RegisterPreviousData(PreviousDataKey: Integer);
begin
  if IsTaskSelected('desktopicon') then
    SetPreviousData(PreviousDataKey, 'desktopicon', '1')
  else
    SetPreviousData(PreviousDataKey, 'desktopicon', '0');
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  Path: string;
  AppPath: string;
  P: Integer;
begin
  if CurUninstallStep = usPostUninstall then
  begin
    if RegQueryStringValue(HKEY_LOCAL_MACHINE,
         'SYSTEM\CurrentControlSet\Control\Session Manager\Environment',
         'Path', Path) then
    begin
      AppPath := ExpandConstant('{app}');
      P := Pos(';' + Uppercase(AppPath), ';' + Uppercase(Path));
      if P > 0 then
      begin
        Delete(Path, P, Length(AppPath) + 1);
        RegWriteExpandStringValue(HKEY_LOCAL_MACHINE,
          'SYSTEM\CurrentControlSet\Control\Session Manager\Environment',
          'Path', Path);
      end;
    end;
  end;
end;
