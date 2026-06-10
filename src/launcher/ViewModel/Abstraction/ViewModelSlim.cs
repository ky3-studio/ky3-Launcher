//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using kyxsan.Core;
using kyxsan.Core.Logging;
using kyxsan.Core.Property;
using kyxsan.Service.Navigation;

namespace kyxsan.ViewModel.Abstraction;

internal abstract partial class ViewModelSlim : ObservableObject
{
    [GeneratedConstructor]
    public partial ViewModelSlim(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial bool IsInitialized { get; set; }

    public IProperty<bool> IsViewUnloaded { get => field ??= Property.Create(false); }

    protected partial IServiceProvider ServiceProvider { get; }

    [Command("LoadCommand")]
    protected virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}

internal abstract partial class ViewModelSlim<TPage> : ViewModelSlim
    where TPage : Page
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial ViewModelSlim(IServiceProvider serviceProvider);

    [Command("NavigateCommand")]
    protected virtual void Navigate()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI($"Navigate to {TypeNameHelper.GetTypeDisplayName(typeof(TPage), fullName: false)}", "ViewModelSlim.Command"));

        INavigationService navigationService = ServiceProvider.GetRequiredService<INavigationService>();
        navigationService.Navigate<TPage>(new NavigationExtraData(new DrillInNavigationTransitionInfo()), true);
    }
}