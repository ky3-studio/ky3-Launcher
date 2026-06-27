//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

namespace Launcher.Core.Setting;

internal static class SettingKeys
{
    // Application
    public const string DataDirectory = "Launcher::Application::DataFolderPath";
    public const string OverrideElevationRequirement = "Launcher::Application::Elevation::Override";
    public const string RunElevated = "Launcher::Application::Elevation::RunElevated";
    public const string StartupEnabled = "Launcher::Application::Startup::Enabled";
    public const string LaunchTimes = "Launcher::Application::LaunchTimes";
    public const string CacheDirectory = "Launcher::Application::CacheFolderPath";
    public const string PreviousDataDirectoryToDelete = "Launcher::Application::PreviousDataFolderToDelete";
    public const string LastVersion = "Launcher::Application::Update::LastVersion";
    public const string AlwaysIsFirstRunAfterUpdate = "Launcher::Application::Update::LastVersion::TreatAsFirstRun";
    public const string OverrideUpdateVersionComparison = "Launcher::Application::Update::VersionComparison::Override";

    // Globalization
    public const string PrimaryLanguage = "Launcher::Globalization::PrimaryLanguage";
    public const string AnnouncementRegion = "Launcher::Globalization::Region::Announcement";
    public const string CalendarServerTimeZoneOffset = "Launcher::Calendar::ServerTimeZoneOffset";
    public const string AnnouncementSeenIds = "Launcher::Announcement::SeenIds";
    public const string ClientRegistered = "Launcher::Backend::ClientRegistered";
    public const string ClientLastHeartbeatDate = "Launcher::Backend::ClientLastHeartbeatDate";
    public const string ClientLastHeartbeatVersion = "Launcher::Backend::ClientLastHeartbeatVersion";

    // UI
    public const string BackgroundImageType = "Launcher::UI::BackgroundImage::Type";
    public const string BackgroundImageCustomPath = "Launcher::UI::BackgroundImage::CustomPath";
    public const string HomePageCardVisible = "Launcher::UI::HomePage::Card::Visible";
    public const string HomePageIndicatorVisible = "Launcher::UI::HomePage::Indicator::Visible";
    public const string BackgroundSwitchInterval = "Launcher::UI::BackgroundImage::SwitchInterval";
    public const string BackgroundShowDynamic = "Launcher::UI::BackgroundImage::ShowDynamic";
    public const string BackgroundShowStatic = "Launcher::UI::BackgroundImage::ShowStatic";
    public const string ElementTheme = "Launcher::UI::ElementTheme";
    public const string SystemBackdropType = "Launcher::UI::SystemBackdropType";
    public const string GuideState = "Launcher::UI::Windowing::GuideWindow::State::1.17";
    public const string LastWindowCloseBehavior = "Launcher::UI::Windowing::LastWindowCloseBehavior";
    public const string IsNavPaneOpen = "Launcher::UI::Windowing::MainWindow::NavigationView::IsPaneOpen";
    public const string RememberWindowSize = "Launcher::UI::Windowing::MainWindow::RememberWindowSize";
    public const string LastWindowWidth = "Launcher::UI::Windowing::MainWindow::LastWindowWidth";
    public const string LastWindowHeight = "Launcher::UI::Windowing::MainWindow::LastWindowHeight";
    public const string LastWindowX = "Launcher::UI::Windowing::MainWindow::LastWindowX";
    public const string LastWindowY = "Launcher::UI::Windowing::MainWindow::LastWindowY";

    // HotKey
    public const string HotKeyRepeatForeverInGameOnly = "Launcher::HotKey::RepeatForever::InGameOnly";
    public const string HotKeyKeyPressRepeatForever = "Launcher::HotKey::RepeatForever::KeyPress";
    public const string HotKeyMouseClickRepeatForever = "Launcher::HotKey::RepeatForever::MouseClick";
    public const string LowLevelKeyboardWebView2VideoPlayPause = "Launcher::HotKey::LowLevel::WebView2::Video::PlayPause";
    public const string LowLevelKeyboardWebView2VideoFastForward = "Launcher::HotKey::LowLevel::WebView2::Video::FastForward";
    public const string LowLevelKeyboardWebView2VideoRewind = "Launcher::HotKey::LowLevel::WebView2::Video::Rewind";
    public const string LowLevelKeyboardWebView2Hide = "Launcher::HotKey::LowLevel::WebView2::Hide";
    public const string LowLevelKeyboardOverlayHide = "Launcher::HotKey::LowLevel::Overlay::Hide";

    // Passport
    public const string PassportRefreshToken = "Launcher::Passport::RefreshToken";
    public const string PassportUserName = "Launcher::Passport::UserName";

    // AvatarProperty
    public const string AvatarPropertySortDescriptionKind = "Launcher::AvatarProperty::SortDescriptionKind";

    // DailyNote
    public const string DailyNoteIsAutoRefreshEnabled = "Launcher::DailyNote::AutoRefresh::Enabled";
    public const string DailyNoteRefreshSeconds = "Launcher::DailyNote::RefreshSeconds";
    public const string DailyNoteReminderNotify = "Launcher::DailyNote::ReminderNotify";
    public const string DailyNoteSilentWhenPlayingGame = "Launcher::DailyNote::SilentWhenPlayingGame";
    public const string DailyNoteWebhookUrl = "Launcher::DailyNote::Webhook::Url";

    // Geetest
    public const string GeetestCustomCompositeUrl = "Launcher::Geetest::CustomCompositeUrl";

    // Game
    public const string LaunchAspectRatios = "Launcher::Game::CommandLine::AspectRatios";
    public const string LaunchUsingHoyolabAccount = "Launcher::Game::CommandLine::AuthTicket";
    public const string LaunchSelectedHoyolabUserMid = "Launcher::Game::CommandLine::AuthTicket::SelectedUserMid";
    public const string LaunchIsBorderless = "Launcher::Game::CommandLine::Borderless";
    public const string LaunchIsWindowed = "Launcher::Game::CommandLine::Windowed";
    public const string LaunchResolutionPresetIndex = "Launcher::Game::CommandLine::ResolutionPresetIndex";
    public const string LaunchAreCommandLineArgumentsEnabled = "Launcher::Game::CommandLine::Enabled";
    public const string LaunchIsExclusive = "Launcher::Game::CommandLine::Exclusive";
    public const string LaunchIsFullScreen = "Launcher::Game::CommandLine::FullScreen";
    public const string LaunchMonitor = "Launcher::Game::CommandLine::Monitor";
    public const string LaunchIsMonitorEnabled = "Launcher::Game::CommandLine::Monitor::Enabled";
    public const string LaunchPlatformType = "Launcher::Game::CommandLine::PlatformType";
    public const string LaunchIsPlatformTypeEnabled = "Launcher::Game::CommandLine::PlatformType::Enabled";
    public const string LaunchScreenHeight = "Launcher::Game::CommandLine::ScreenHeight";
    public const string LaunchIsScreenHeightEnabled = "Launcher::Game::CommandLine::ScreenHeight::Enabled";
    public const string LaunchScreenWidth = "Launcher::Game::CommandLine::ScreenWidth";
    public const string LaunchIsScreenWidthEnabled = "Launcher::Game::CommandLine::ScreenWidth::Enabled";
    public const string LaunchUsingStarwardPlayTimeStatistics = "Launcher::Game::InterProcess::Starward::PlayTimeStatistics";
    public const string LaunchUsingBetterGenshinImpactAutomation = "Launcher::Game::InterProcess::BetterGenshinImpact::Automation";
    public const string LaunchBgiPath = "Launcher::Game::InterProcess::BetterGenshinImpact::Path";
    public const string LaunchBgiDelay = "Launcher::Game::InterProcess::BetterGenshinImpact::Delay";
    public const string LaunchBgiArgs = "Launcher::Game::InterProcess::BetterGenshinImpact::Args";
    public const string LaunchAttachProgramEnabled = "Launcher::Game::InterProcess::AttachProgram::Enabled";
    public const string LaunchAttachProgramPath = "Launcher::Game::InterProcess::AttachProgram::Path";
    public const string LaunchAttachProgramDelay = "Launcher::Game::InterProcess::AttachProgram::Delay";
    public const string LaunchAttachProgramArgs = "Launcher::Game::InterProcess::AttachProgram::Args";
    public const string LaunchAttachProgram2Enabled = "Launcher::Game::InterProcess::AttachProgram2::Enabled";
    public const string LaunchAttachProgram2Path = "Launcher::Game::InterProcess::AttachProgram2::Path";
    public const string LaunchAttachProgram2Delay = "Launcher::Game::InterProcess::AttachProgram2::Delay";
    public const string LaunchAttachProgram2Args = "Launcher::Game::InterProcess::AttachProgram2::Args";
    public const string LaunchAttachProgram3Enabled = "Launcher::Game::InterProcess::AttachProgram3::Enabled";
    public const string LaunchAttachProgram3Path = "Launcher::Game::InterProcess::AttachProgram3::Path";
    public const string LaunchAttachProgram3Delay = "Launcher::Game::InterProcess::AttachProgram3::Delay";
    public const string LaunchAttachProgram3Args = "Launcher::Game::InterProcess::AttachProgram3::Args";
    public const string LaunchDisableShowDamageText = "Launcher::Game::Island::DamageText::Show";
    public const string LaunchIsIslandEnabled = "Launcher::Game::Island::Enabled";
    public const string LaunchIsIslandRiskAccepted = "Launcher::Game::Island::RiskAccepted";
    public const string LaunchDisableEventCameraMove = "Launcher::Game::Island::Event::CameraMove::Disabled";
    public const string LaunchTargetFov = "Launcher::Game::Island::FieldOfView";
    public const string LaunchIsSetFieldOfViewEnabled = "Launcher::Game::Island::FieldOfView::Enabled";
    public const string LaunchFixLowFovScene = "Launcher::Game::Island::FieldOfView::FixLowFovScene";
    public const string LaunchDisableFogRendering = "Launcher::Game::Island::FieldOfView::DisableFogRendering";
    public const string LaunchUsingTouchScreen = "Launcher::Game::Island::InputDevice::TouchScreen";
    public const string LaunchRemoveOpenTeamProgress = "Launcher::Game::Island::OpenTeamProgress::Remove";
    public const string LaunchHideQuestBanner = "Launcher::Game::Island::QuestBanner::Hide";
    public const string LaunchResinListItemId000106Allowed = "Launcher::Game::Island::Reward::000106";
    public const string LaunchResinListItemId000201Allowed = "Launcher::Game::Island::Reward::000201";
    public const string LaunchResinListItemId107009Allowed = "Launcher::Game::Island::Reward::107009";
    public const string LaunchResinListItemId107012Allowed = "Launcher::Game::Island::Reward::107012";
    public const string LaunchResinListItemId220007Allowed = "Launcher::Game::Island::Reward::220007";
    public const string LaunchRedirectCombineEntry = "Launcher::Game::Island::Synthesis::Redirect";
    public const string LaunchCraftKey = "Launcher::Game::Island::Synthesis::CraftKey";
    public const string LaunchCraftModifier = "Launcher::Game::Island::Synthesis::CraftModifier";
    public const string LaunchDisableCharFade = "Launcher::Game::Island::CharFade::Disabled";
    public const string LaunchHideUID = "Launcher::Game::Island::UID::Hide";
    public const string LaunchHideMenuUID = "Launcher::Game::Island::MenuUID::Hide";
    public const string LaunchDisableVSync = "Launcher::Game::Island::VSync::Disabled";
    public const string LaunchEnableFps = "Launcher::Game::Island::Fps::Enable";
    public const string LaunchTargetFps = "Launcher::Game::Island::Fps::FrameRate";
    public const string LaunchEnableCraftRedirect = "Launcher::Game::Island::Synthesis::AutoRedirect";
    public const string LaunchEnableDispatch = "Launcher::Game::Island::Dispatch::Enable";
    public const string LaunchRedirectDispatch = "Launcher::Game::Island::Dispatch::Redirect";
    public const string LaunchDispatchKey = "Launcher::Game::Island::Dispatch::Key";
    public const string LaunchDispatchModifier = "Launcher::Game::Island::Dispatch::Modifier";
    public const string LaunchEnableCooking = "Launcher::Game::Island::Cooking::Enable";
    public const string LaunchCookingKey = "Launcher::Game::Island::Cooking::Key";
    public const string LaunchCookingModifier = "Launcher::Game::Island::Cooking::Modifier";
    public const string LaunchEnableForge = "Launcher::Game::Island::Forge::Enable";
    public const string LaunchForgeKey = "Launcher::Game::Island::Forge::Key";
    public const string LaunchForgeModifier = "Launcher::Game::Island::Forge::Modifier";
    public const string LaunchEnableNoGrass = "Launcher::Game::Island::NoGrass::Enable";
    public const string LaunchEnableGui = "Launcher::Game::Island::Gui::Enable";
    public const string LaunchGuiKey = "Launcher::Game::Island::Gui::Key";
    public const string LaunchGuiModifier = "Launcher::Game::Island::Gui::Modifier";


    // Custom DLL Injection
    public const string LaunchCustomDllConfigs = "Launcher::Game::CustomDll::Configs";

    public const string LaunchUsingOverlay = "Launcher::Game::Overlay";
    public const string LaunchOverlaySelectedCatalogId = "Launcher::Game::Overlay::CatalogId";
    public const string LaunchOverlayWindowIsVisible = "Launcher::Game::Overlay::Visible";
    public const string EnableBetaGameInstall = "Launcher::Game::Package::BetaGame::Enable";
    public const string LaunchOverridePackageConvertDirectoryPermissions = "Launcher::Game::Package::Convert::Directory::Permissions::Override";
    public const string OverridePhysicalDriverType = "Launcher::Game::Package::PhysicalDriver::Type::Override";
    public const string PhysicalDriverIsAlwaysSolidState = "Launcher::Game::Package::PhysicalDriver::Type::IsSolidState";
    public const string TreatPredownloadAsMain = "Launcher::Game::Package::Predownload::TreatAsMain";
    public const string LaunchGamePath = "Launcher::Game::Path";
    public const string LaunchGamePathEntries = "Launcher::Game::Path::Entries";
    public const string LaunchIsWindowsHDREnabled = "Launcher::Game::Registry::WindowsHDR::Enabled";

    // Advanced Start
    public const string LaunchAdvancedStartProgramPath = "Launcher::Game::AdvancedStart::ProgramPath";
    public const string LaunchAdvancedStartFeedEndpoint = "Launcher::Game::AdvancedStart::FeedEndpoint";
    public const string LaunchAdvancedStartDelayedPrograms = "Launcher::Game::AdvancedStart::DelayedPrograms";
    public const string LaunchAdvancedStartDelayedOnAdvancedStart = "Launcher::Game::AdvancedStart::DelayedOnAdvancedStart";
    public const string LaunchAdvancedStartDelayedOnGameLaunch = "Launcher::Game::AdvancedStart::DelayedOnGameLaunch";

    // Storage

    // Web
    public const string BridgeShareSaveType = "Launcher::Web::WebView::BridgeShare::SaveType";
    public const string WebView2VideoFastForwardOrRewindSeconds = "Launcher::Web::WebView::Video::FastForwardOrRewind::Seconds";
}