//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.ExceptionService;
using kyxsan.Model.Entity;
using kyxsan.Service.DailyNote;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Notification;
using kyxsan.UI.Xaml.View.Page;
using System.Collections.ObjectModel;

namespace kyxsan.ViewModel.DailyNote;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class DailyNoteViewModelSlim : Abstraction.ViewModelSlim<DailyNotePage>
{
    private readonly IDailyNoteService dailyNoteService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private DailyNoteMetadataContext? metadataContext;

    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial DailyNoteViewModelSlim(IServiceProvider serviceProvider);

    // This property must be a reference type
    [ObservableProperty]
    public partial List<DailyNoteEntry>? DailyNoteEntries { get; set; }

    /// <inheritdoc/>
    protected override async Task LoadAsync()
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return;
        }

        metadataContext = await metadataService.GetContextAsync<DailyNoteMetadataContext>().ConfigureAwait(false);

        try
        {
            await taskContext.SwitchToBackgroundAsync();
            ObservableCollection<DailyNoteEntry> entries = await dailyNoteService
                .GetDailyNoteEntryCollectionAsync(metadataContext)
                .ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();

            // We must make a copy of the entries collection to avoid the following exception:
            // Element is already the child of another element.
            DailyNoteEntries = [.. entries];
            IsInitialized = true;
        }
        catch (kyxsanException ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }
}