//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using kyxsan.Core.IO;
using kyxsan.Core.LifeCycle;
using kyxsan.Factory.Picker;
using kyxsan.Model;
using kyxsan.Service;
using kyxsan.Service.BackgroundImage;
using kyxsan.UI.Xaml;
using kyxsan.UI.Xaml.Control.Theme;
using kyxsan.UI.Xaml.Media.Backdrop;

namespace kyxsan.ViewModel.Setting;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingAppearanceViewModel : Abstraction.ViewModel
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial SettingAppearanceViewModel(IServiceProvider serviceProvider);

    public partial CultureOptions CultureOptions { get; }

    public partial AppOptions AppOptions { get; }

    public NameCultureInfoValue? SelectedCulture
    {
        get => field ??= Selection.Initialize(CultureOptions.Cultures, CultureOptions.CurrentCulture.Value) ?? CultureOptions.Cultures.FirstOrDefault();
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                CultureOptions.CurrentCulture.Value = value.Value;
                AppInstance.Restart(string.Empty);
            }
        }
    }

    public NameValue<BackdropType>? SelectedBackdropType
    {
        get => field ??= Selection.Initialize(AppOptions.BackdropTypes, AppOptions.BackdropType.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.BackdropType.Value = value.Value;
            }
        }
    }

    public NameValue<ElementTheme>? SelectedElementTheme
    {
        get => field ??= Selection.Initialize(AppOptions.LazyElementThemes.Value, AppOptions.ElementTheme.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.ElementTheme.Value = value.Value;

                try
                {
                    FrameworkTheming.SetTheme(ThemeHelper.ElementToFramework(value.Value));
                }
                catch
                {
                }

                try
                {
                    ICurrentXamlWindowReference windowRef = serviceProvider.GetRequiredService<ICurrentXamlWindowReference>();
                    if (windowRef.Window?.Content is FrameworkElement rootElement)
                    {
                        rootElement.RequestedTheme = value.Value;
                    }
                }
                catch
                {
                }
            }
        }
    }

    public NameValue<BackgroundImageType>? SelectedBackgroundImageType
    {
        get
        {
            if (field is null)
            {
                field = Selection.Initialize(AppOptions.BackgroundImageTypes, AppOptions.BackgroundImageType.Value);
                IsCustomBackgroundSelected = field?.Value == BackgroundImageType.Custom;
            }

            return field;
        }
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.BackgroundImageType.Value = value.Value;
                IsCustomBackgroundSelected = value.Value == BackgroundImageType.Custom;
            }
        }
    }

    public bool IsCustomBackgroundSelected
    {
        get => field;
        set
        {
            if (SetProperty(ref field, value))
            {
                OnPropertyChanged(nameof(CustomBackgroundDisplayPath));
                OnPropertyChanged(nameof(HasCustomBackgroundPath));
            }
        }
    }

    public bool HasCustomBackgroundPath
    {
        get => IsCustomBackgroundSelected && !string.IsNullOrEmpty(AppOptions.BackgroundImageCustomPath.Value);
    }

    public string CustomBackgroundDisplayPath
    {
        get
        {
            if (!IsCustomBackgroundSelected)
            {
                return SH.ViewPageSettingBackgroundImageCustomHint;
            }

            string path = AppOptions.BackgroundImageCustomPath.Value;
            return string.IsNullOrEmpty(path) ? SH.ViewPageSettingBackgroundImageCustomNone : path;
        }
    }

    [Command("PickCustomBackgroundCommand")]
    private void PickCustomBackground()
    {
        IFileSystemPickerInteraction picker = serviceProvider.GetRequiredService<IFileSystemPickerInteraction>();
        (bool picked, ValueFile file) = picker.PickFile(
            SH.ViewPageSettingBackgroundImageHeader,
            null,
            SH.ServiceBackgroundImageTypeCustom,
            "*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.webp;*.mp4;*.webm;*.wmv;*.avi");

        if (picked && file.HasValue)
        {
            AppOptions.BackgroundImageCustomPath.Value = (string)file;
            OnPropertyChanged(nameof(CustomBackgroundDisplayPath));
            OnPropertyChanged(nameof(HasCustomBackgroundPath));
        }
    }

    [Command("ClearCustomBackgroundCommand")]
    private void ClearCustomBackground()
    {
        AppOptions.BackgroundImageCustomPath.Value = string.Empty;
        OnPropertyChanged(nameof(CustomBackgroundDisplayPath));
        OnPropertyChanged(nameof(HasCustomBackgroundPath));
    }

    public NameValue<int>? SelectedBackgroundSwitchInterval
    {
        get => field ??= Selection.Initialize(AppOptions.BackgroundSwitchIntervals, AppOptions.BackgroundSwitchInterval.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.BackgroundSwitchInterval.Value = value.Value;
            }
        }
    }

    public NameValue<LastWindowCloseBehavior>? SelectedLastWindowCloseBehavior
    {
        get => field ??= Selection.Initialize(AppOptions.LastWindowCloseBehaviors, AppOptions.LastWindowCloseBehavior.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.LastWindowCloseBehavior.Value = value.Value;
            }
        }
    }
}