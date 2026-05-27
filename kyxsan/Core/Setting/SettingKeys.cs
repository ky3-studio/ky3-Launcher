//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

namespace kyxsan.Core.Setting;

internal static class SettingKeys
{
    // Application
    public const string DataDirectory                   = "kyxsan::Application::DataFolderPath";
    public const string OverrideElevationRequirement    = "kyxsan::Application::Elevation::Override";
    public const string RunElevated                     = "kyxsan::Application::Elevation::RunElevated";
    public const string StartupEnabled                  = "kyxsan::Application::Startup::Enabled";
    public const string LaunchTimes                     = "kyxsan::Application::LaunchTimes";
    public const string CacheDirectory                   = "kyxsan::Application::CacheFolderPath";
    public const string PreviousDataDirectoryToDelete   = "kyxsan::Application::PreviousDataFolderToDelete";
    public const string LastVersion                     = "kyxsan::Application::Update::LastVersion";
    public const string AlwaysIsFirstRunAfterUpdate     = "kyxsan::Application::Update::LastVersion::TreatAsFirstRun";
    public const string OverrideUpdateVersionComparison = "kyxsan::Application::Update::VersionComparison::Override";

    // Globalization
    public const string PrimaryLanguage              = "kyxsan::Globalization::PrimaryLanguage";
    public const string AnnouncementRegion           = "kyxsan::Globalization::Region::Announcement";
    public const string AnnouncementSeenIds          = "kyxsan::Announcement::SeenIds";
    public const string ClientRegistered             = "kyxsan::Backend::ClientRegistered";
    public const string ClientLastHeartbeatDate      = "kyxsan::Backend::ClientLastHeartbeatDate";

    // UI
    public const string BackgroundImageType          = "kyxsan::UI::BackgroundImage::Type";
    public const string BackgroundImageCustomPath      = "kyxsan::UI::BackgroundImage::CustomPath";
    public const string HomePageCardVisible            = "kyxsan::UI::HomePage::Card::Visible";
    public const string HomePageIndicatorVisible       = "kyxsan::UI::HomePage::Indicator::Visible";
    public const string BackgroundSwitchInterval        = "kyxsan::UI::BackgroundImage::SwitchInterval";
    public const string BackgroundShowDynamic           = "kyxsan::UI::BackgroundImage::ShowDynamic";
    public const string BackgroundShowStatic            = "kyxsan::UI::BackgroundImage::ShowStatic";
    public const string ElementTheme                 = "kyxsan::UI::ElementTheme";
    public const string SystemBackdropType           = "kyxsan::UI::SystemBackdropType";
    public const string GuideState                   = "kyxsan::UI::Windowing::GuideWindow::State::1.17";
    public const string LastWindowCloseBehavior      = "kyxsan::UI::Windowing::LastWindowCloseBehavior";
    public const string IsNavPaneOpen                = "kyxsan::UI::Windowing::MainWindow::NavigationView::IsPaneOpen";
    public const string RememberWindowSize            = "kyxsan::UI::Windowing::MainWindow::RememberWindowSize";
    public const string LastWindowWidth               = "kyxsan::UI::Windowing::MainWindow::LastWindowWidth";
    public const string LastWindowHeight              = "kyxsan::UI::Windowing::MainWindow::LastWindowHeight";
    public const string LastWindowX                   = "kyxsan::UI::Windowing::MainWindow::LastWindowX";
    public const string LastWindowY                   = "kyxsan::UI::Windowing::MainWindow::LastWindowY";

    // HotKey
    public const string HotKeyRepeatForeverInGameOnly            = "kyxsan::HotKey::RepeatForever::InGameOnly";
    public const string HotKeyKeyPressRepeatForever              = "kyxsan::HotKey::RepeatForever::KeyPress";
    public const string HotKeyMouseClickRepeatForever            = "kyxsan::HotKey::RepeatForever::MouseClick";
    public const string LowLevelKeyboardWebView2VideoPlayPause   = "kyxsan::HotKey::LowLevel::WebView2::Video::PlayPause";
    public const string LowLevelKeyboardWebView2VideoFastForward = "kyxsan::HotKey::LowLevel::WebView2::Video::FastForward";
    public const string LowLevelKeyboardWebView2VideoRewind      = "kyxsan::HotKey::LowLevel::WebView2::Video::Rewind";
    public const string LowLevelKeyboardWebView2Hide             = "kyxsan::HotKey::LowLevel::WebView2::Hide";
    public const string LowLevelKeyboardOverlayHide              = "kyxsan::HotKey::LowLevel::Overlay::Hide";

    // Passport
    public const string PassportRefreshToken = "kyxsan::Passport::RefreshToken";
    public const string PassportUserName     = "kyxsan::Passport::UserName";

    // AvatarProperty
    public const string AvatarPropertySortDescriptionKind = "kyxsan::AvatarProperty::SortDescriptionKind";

    // DailyNote
    public const string DailyNoteIsAutoRefreshEnabled  = "kyxsan::DailyNote::AutoRefresh::Enabled";
    public const string DailyNoteRefreshSeconds        = "kyxsan::DailyNote::RefreshSeconds";
    public const string DailyNoteReminderNotify        = "kyxsan::DailyNote::ReminderNotify";
    public const string DailyNoteSilentWhenPlayingGame = "kyxsan::DailyNote::SilentWhenPlayingGame";
    public const string DailyNoteWebhookUrl            = "kyxsan::DailyNote::Webhook::Url";

    // Geetest
    public const string GeetestCustomCompositeUrl = "kyxsan::Geetest::CustomCompositeUrl";

    // Game
    public const string LaunchAspectRatios                               = "kyxsan::Game::CommandLine::AspectRatios";
    public const string LaunchUsingHoyolabAccount                        = "kyxsan::Game::CommandLine::AuthTicket";
    public const string LaunchSelectedHoyolabUserMid                     = "kyxsan::Game::CommandLine::AuthTicket::SelectedUserMid";
    public const string LaunchIsBorderless                               = "kyxsan::Game::CommandLine::Borderless";
    public const string LaunchIsWindowed                                   = "kyxsan::Game::CommandLine::Windowed";
    public const string LaunchResolutionPresetIndex                        = "kyxsan::Game::CommandLine::ResolutionPresetIndex";
    public const string LaunchAreCommandLineArgumentsEnabled             = "kyxsan::Game::CommandLine::Enabled";
    public const string LaunchIsExclusive                                = "kyxsan::Game::CommandLine::Exclusive";
    public const string LaunchIsFullScreen                               = "kyxsan::Game::CommandLine::FullScreen";
    public const string LaunchMonitor                                    = "kyxsan::Game::CommandLine::Monitor";
    public const string LaunchIsMonitorEnabled                           = "kyxsan::Game::CommandLine::Monitor::Enabled";
    public const string LaunchPlatformType                               = "kyxsan::Game::CommandLine::PlatformType";
    public const string LaunchIsPlatformTypeEnabled                      = "kyxsan::Game::CommandLine::PlatformType::Enabled";
    public const string LaunchScreenHeight                               = "kyxsan::Game::CommandLine::ScreenHeight";
    public const string LaunchIsScreenHeightEnabled                      = "kyxsan::Game::CommandLine::ScreenHeight::Enabled";
    public const string LaunchScreenWidth                                = "kyxsan::Game::CommandLine::ScreenWidth";
    public const string LaunchIsScreenWidthEnabled                       = "kyxsan::Game::CommandLine::ScreenWidth::Enabled";
    public const string LaunchUsingStarwardPlayTimeStatistics            = "kyxsan::Game::InterProcess::Starward::PlayTimeStatistics";
    public const string LaunchUsingBetterGenshinImpactAutomation         = "kyxsan::Game::InterProcess::BetterGenshinImpact::Automation";
    public const string LaunchBgiPath                                    = "kyxsan::Game::InterProcess::BetterGenshinImpact::Path";
    public const string LaunchBgiDelay                                   = "kyxsan::Game::InterProcess::BetterGenshinImpact::Delay";
    public const string LaunchBgiArgs                                    = "kyxsan::Game::InterProcess::BetterGenshinImpact::Args";
    public const string LaunchAttachProgramEnabled                       = "kyxsan::Game::InterProcess::AttachProgram::Enabled";
    public const string LaunchAttachProgramPath                          = "kyxsan::Game::InterProcess::AttachProgram::Path";
    public const string LaunchAttachProgramDelay                         = "kyxsan::Game::InterProcess::AttachProgram::Delay";
    public const string LaunchAttachProgramArgs                          = "kyxsan::Game::InterProcess::AttachProgram::Args";
    public const string LaunchAttachProgram2Enabled                      = "kyxsan::Game::InterProcess::AttachProgram2::Enabled";
    public const string LaunchAttachProgram2Path                         = "kyxsan::Game::InterProcess::AttachProgram2::Path";
    public const string LaunchAttachProgram2Delay                        = "kyxsan::Game::InterProcess::AttachProgram2::Delay";
    public const string LaunchAttachProgram2Args                         = "kyxsan::Game::InterProcess::AttachProgram2::Args";
    public const string LaunchAttachProgram3Enabled                      = "kyxsan::Game::InterProcess::AttachProgram3::Enabled";
    public const string LaunchAttachProgram3Path                         = "kyxsan::Game::InterProcess::AttachProgram3::Path";
    public const string LaunchAttachProgram3Delay                        = "kyxsan::Game::InterProcess::AttachProgram3::Delay";
    public const string LaunchAttachProgram3Args                         = "kyxsan::Game::InterProcess::AttachProgram3::Args";
    public const string LaunchDisableShowDamageText                      = "kyxsan::Game::Island::DamageText::Show";
    public const string LaunchIsIslandEnabled                            = "kyxsan::Game::Island::Enabled";
    public const string LaunchIsIslandRiskAccepted                       = "kyxsan::Game::Island::RiskAccepted";
    public const string LaunchDisableEventCameraMove                     = "kyxsan::Game::Island::Event::CameraMove::Disabled";
    public const string LaunchTargetFov                                  = "kyxsan::Game::Island::FieldOfView";
    public const string LaunchIsSetFieldOfViewEnabled                    = "kyxsan::Game::Island::FieldOfView::Enabled";
    public const string LaunchFixLowFovScene                             = "kyxsan::Game::Island::FieldOfView::FixLowFovScene";
    public const string LaunchDisableFogRendering                        = "kyxsan::Game::Island::FieldOfView::DisableFogRendering";
    public const string LaunchUsingTouchScreen                           = "kyxsan::Game::Island::InputDevice::TouchScreen";
    public const string LaunchRemoveOpenTeamProgress                     = "kyxsan::Game::Island::OpenTeamProgress::Remove";
    public const string LaunchHideQuestBanner                            = "kyxsan::Game::Island::QuestBanner::Hide";
    public const string LaunchResinListItemId000106Allowed               = "kyxsan::Game::Island::Reward::000106";
    public const string LaunchResinListItemId000201Allowed               = "kyxsan::Game::Island::Reward::000201";
    public const string LaunchResinListItemId107009Allowed               = "kyxsan::Game::Island::Reward::107009";
    public const string LaunchResinListItemId107012Allowed               = "kyxsan::Game::Island::Reward::107012";
    public const string LaunchResinListItemId220007Allowed               = "kyxsan::Game::Island::Reward::220007";
    public const string LaunchRedirectCombineEntry                       = "kyxsan::Game::Island::Synthesis::Redirect";
    public const string LaunchCraftKey                                   = "kyxsan::Game::Island::Synthesis::CraftKey";
    public const string LaunchCraftModifier                              = "kyxsan::Game::Island::Synthesis::CraftModifier";
    public const string LaunchDisableCharFade                            = "kyxsan::Game::Island::CharFade::Disabled";
    public const string LaunchHideUID                                    = "kyxsan::Game::Island::UID::Hide";
    public const string LaunchHideMenuUID                                = "kyxsan::Game::Island::MenuUID::Hide";
    public const string LaunchDisableVSync                               = "kyxsan::Game::Island::VSync::Disabled";
    public const string LaunchEnableFps                                 = "kyxsan::Game::Island::Fps::Enable";
    public const string LaunchTargetFps                                  = "kyxsan::Game::Island::Fps::FrameRate";
    public const string LaunchEnableCraftRedirect                        = "kyxsan::Game::Island::Synthesis::AutoRedirect";
    public const string LaunchEnableDispatch                             = "kyxsan::Game::Island::Dispatch::Enable";
    public const string LaunchRedirectDispatch                           = "kyxsan::Game::Island::Dispatch::Redirect";
    public const string LaunchDispatchKey                                = "kyxsan::Game::Island::Dispatch::Key";
    public const string LaunchDispatchModifier                           = "kyxsan::Game::Island::Dispatch::Modifier";
    public const string LaunchEnableCooking                              = "kyxsan::Game::Island::Cooking::Enable";
    public const string LaunchCookingKey                                 = "kyxsan::Game::Island::Cooking::Key";
    public const string LaunchCookingModifier                            = "kyxsan::Game::Island::Cooking::Modifier";
    public const string LaunchEnableForge                                = "kyxsan::Game::Island::Forge::Enable";
    public const string LaunchForgeKey                                   = "kyxsan::Game::Island::Forge::Key";
    public const string LaunchForgeModifier                             = "kyxsan::Game::Island::Forge::Modifier";


    // Custom DLL Injection
    public const string LaunchCustomDllConfigs                           = "kyxsan::Game::CustomDll::Configs";

    public const string LaunchUsingOverlay                               = "kyxsan::Game::Overlay";
    public const string LaunchOverlaySelectedCatalogId                   = "kyxsan::Game::Overlay::CatalogId";
    public const string LaunchOverlayWindowIsVisible                     = "kyxsan::Game::Overlay::Visible";
    public const string EnableBetaGameInstall                            = "kyxsan::Game::Package::BetaGame::Enable";
    public const string LaunchOverridePackageConvertDirectoryPermissions = "kyxsan::Game::Package::Convert::Directory::Permissions::Override";
    public const string OverridePhysicalDriverType                       = "kyxsan::Game::Package::PhysicalDriver::Type::Override";
    public const string PhysicalDriverIsAlwaysSolidState                 = "kyxsan::Game::Package::PhysicalDriver::Type::IsSolidState";
    public const string TreatPredownloadAsMain                           = "kyxsan::Game::Package::Predownload::TreatAsMain";
    public const string LaunchGamePath                                   = "kyxsan::Game::Path";
    public const string LaunchGamePathEntries                            = "kyxsan::Game::Path::Entries";
    public const string LaunchIsWindowsHDREnabled                        = "kyxsan::Game::Registry::WindowsHDR::Enabled";

    // Advanced Start
    public const string LaunchAdvancedStartProgramPath                   = "kyxsan::Game::AdvancedStart::ProgramPath";
    public const string LaunchAdvancedStartFeedEndpoint                  = "kyxsan::Game::AdvancedStart::FeedEndpoint";
    public const string LaunchAdvancedStartDelayedPrograms               = "kyxsan::Game::AdvancedStart::DelayedPrograms";
    public const string LaunchAdvancedStartDelayedOnAdvancedStart        = "kyxsan::Game::AdvancedStart::DelayedOnAdvancedStart";
    public const string LaunchAdvancedStartDelayedOnGameLaunch           = "kyxsan::Game::AdvancedStart::DelayedOnGameLaunch";

    // Storage

    // Web
    public const string BridgeShareSaveType                     = "kyxsan::Web::WebView::BridgeShare::SaveType";
    public const string WebView2VideoFastForwardOrRewindSeconds = "kyxsan::Web::WebView::Video::FastForwardOrRewind::Seconds";
}