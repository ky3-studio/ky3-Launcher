//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using kyxsan.Core.Graphics;
using Windows.Graphics;

namespace kyxsan.UI.Xaml.View.Window;

internal sealed partial class IdentifyMonitorWindow : Microsoft.UI.Xaml.Window
{
    public IdentifyMonitorWindow(DisplayArea displayArea, int index)
    {
        InitializeComponent();
        Monitor = $"{displayArea.DisplayId.Value:X8}:{index}";

        OverlappedPresenter presenter = OverlappedPresenter.CreateForContextMenu();
        presenter.IsAlwaysOnTop = true;
        presenter.SetBorderAndTitleBar(false, false);
        AppWindow.SetPresenter(presenter);
        PointInt32 point = new(40, 32);
        SizeInt32 size = displayArea.WorkArea.SizeInt32.Scale(0.1);
        AppWindow.MoveAndResize(RectInt32Convert.RectInt32(point, size), displayArea);
    }

    public string Monitor { get; }

    public static async ValueTask IdentifyAllMonitorsAsync(int secondsDelay)
    {
        List<IdentifyMonitorWindow> windows = [];

        IReadOnlyList<DisplayArea> displayAreas = DisplayArea.FindAll();
        for (int i = 0; i < displayAreas.Count; i++)
        {
            windows.Add(new(displayAreas[i], i + 1));
        }

        foreach (IdentifyMonitorWindow window in windows)
        {
            window.AppWindow.Show(true);
            window.AppWindow.MoveInZOrderAtTop();
        }

        await Task.Delay(TimeSpan.FromSeconds(secondsDelay)).ConfigureAwait(true);

        foreach (IdentifyMonitorWindow window in windows)
        {
            window.Close();
        }

        windows.Clear();
    }
}