//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using kyxsan.Core;
using kyxsan.Factory.ContentDialog;
using System.Collections.Immutable;
using System.IO;

namespace kyxsan.UI.Xaml.View.Dialog;

internal sealed partial class LaunchGameCleanResourceDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;

    public List<CleanResourceItem> CleanableResources { get; }

    public LaunchGameCleanResourceDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();

        CleanableResources = BuildResourceList();
        IsPrimaryButtonEnabled = false;
        UpdateTotalSize();
    }

    public async ValueTask<ImmutableArray<CleanResourceItem>> GetSelectedItemsAsync()
    {
        if (await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false) is ContentDialogResult.Primary)
        {
            await contentDialogFactory.TaskContext.SwitchToMainThreadAsync();
            return ResourceListView.SelectedItems
                .OfType<CleanResourceItem>()
                .Where(item => item.Size > 0)
                .ToImmutableArray();
        }

        return [];
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        long selectedSize = ResourceListView.SelectedItems
            .OfType<CleanResourceItem>()
            .Where(item => item.Size > 0)
            .Sum(item => item.Size);

        IsPrimaryButtonEnabled = selectedSize > 0;
        UpdateTotalSize();
    }

    private void UpdateTotalSize()
    {
        long selectedSize = ResourceListView.SelectedItems
            .OfType<CleanResourceItem>()
            .Where(item => item.Size > 0)
            .Sum(item => item.Size);

        TotalSizeText.Text = selectedSize > 0
            ? string.Format(SH.ViewDialogCleanResourceSelectedText, FormatSize(selectedSize))
            : string.Empty;
    }

    private static List<CleanResourceItem> BuildResourceList()
    {
        List<CleanResourceItem> items = [];
        string serverCacheDir = kyxsanRuntime.GetDataServerCacheDirectory();

        // 转换下载分块缓存 (ServerCache\Chunks)
        string chunksDir = Path.Combine(serverCacheDir, "Chunks");
        items.Add(new CleanResourceItem
        {
            Name = SH.ViewDialogCleanResourceChunksName,
            Path = chunksDir,
            Size = CalculateDirectorySize(chunksDir),
        });

        // 国际服备份资源 (ServerCache\Oversea)
        string overseaDir = Path.Combine(serverCacheDir, "Oversea");
        items.Add(new CleanResourceItem
        {
            Name = SH.ViewDialogCleanResourceOverseaName,
            Path = overseaDir,
            Size = CalculateDirectorySize(overseaDir),
        });

        // 国服备份资源 (ServerCache\Chinese)
        string chineseDir = Path.Combine(serverCacheDir, "Chinese");
        items.Add(new CleanResourceItem
        {
            Name = SH.ViewDialogCleanResourceChineseName,
            Path = chineseDir,
            Size = CalculateDirectorySize(chineseDir),
        });

        return items;
    }

    private static long CalculateDirectorySize(string path)
    {
        if (!Directory.Exists(path))
        {
            return 0;
        }

        long size = 0;
        try
        {
            foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                size += new FileInfo(file).Length;
            }
        }
        catch
        {
        }

        return size;
    }

    internal static string FormatSize(long bytes)
    {
        if (bytes >= 1024L * 1024 * 1024)
        {
            return $"{bytes / (1024.0 * 1024.0 * 1024.0):F2} GB";
        }

        if (bytes >= 1024L * 1024)
        {
            return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }

        if (bytes >= 1024)
        {
            return $"{bytes / 1024.0:F2} KB";
        }

        return $"{bytes} B";
    }
}

internal sealed class CleanResourceItem
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public long Size { get; set; }

    public string SizeText => Size > 0 ? LaunchGameCleanResourceDialog.FormatSize(Size) : SH.ViewDialogCleanResourceNoSize;

    public Brush SizeForeground => Size > 0
        ? new SolidColorBrush(Microsoft.UI.Colors.Orange)
        : (Brush)Application.Current.Resources["TextFillColorTertiaryBrush"];
}
