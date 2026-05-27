//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using JetBrains.Annotations;
using kyxsan.Factory.Progress;

namespace kyxsan.Factory.ContentDialog;

internal sealed class BlockDeferral : IDisposable
{
    private readonly Microsoft.UI.Xaml.Controls.ContentDialog contentDialog;

    private bool disposed;

    public BlockDeferral(Microsoft.UI.Xaml.Controls.ContentDialog contentDialog)
    {
        this.contentDialog = contentDialog;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        contentDialog.DispatcherQueue.Invoke(contentDialog.Hide);
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class BlockDeferral<T> : IDisposable
{
    private readonly IServiceScope serviceScope;
    private readonly BlockDeferral blockDeferral;

    private BlockDeferral(IServiceScope serviceScope, BlockDeferral blockDeferral, IProgress<T> progress)
    {
        this.serviceScope = serviceScope;
        this.blockDeferral = blockDeferral;
        Progress = progress;
    }

    public IProgress<T> Progress { get; }

    public static async ValueTask<BlockDeferral<T>> CreateAsync<TContentDialog>(IServiceProvider serviceProvider, [RequireStaticDelegate] Action<T, TContentDialog> progressHandler)
        where TContentDialog : Microsoft.UI.Xaml.Controls.ContentDialog
    {
        IServiceScope scope = serviceProvider.CreateScope();
        IContentDialogFactory dialogFactory = scope.ServiceProvider.GetRequiredService<IContentDialogFactory>();
        TContentDialog dialog = await dialogFactory.CreateInstanceAsync<TContentDialog>(scope.ServiceProvider).ConfigureAwait(false);
        BlockDeferral dialogScope = await dialogFactory.BlockAsync(dialog).ConfigureAwait(false);

        IProgressFactory progressFactory = scope.ServiceProvider.GetRequiredService<IProgressFactory>();
        IProgress<T> progress = progressFactory.CreateForMainThread(progressHandler, dialog);

        return new(scope, dialogScope, progress);
    }

    public void Dispose()
    {
        blockDeferral.Dispose();
        serviceScope.Dispose();
    }
}