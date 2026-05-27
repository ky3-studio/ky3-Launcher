//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using kyxsan.Core.ExceptionService;
using kyxsan.Core.Graphics;
using kyxsan.Core.Logging;
using kyxsan.Factory.Process;
using kyxsan.Service.kyxsan;
using kyxsan.UI.Windowing;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Graphics;

namespace kyxsan.UI.Xaml.View.Window;

internal sealed partial class ExceptionWindow : Microsoft.UI.Xaml.Window, INotifyPropertyChanged
{
    private readonly SentryId associatedEventId;

    public ExceptionWindow(SentryId associatedEventId, Exception ex)
    {
        // Message pump will die if we introduce XamlWindowController
        InitializeComponent();
        this.associatedEventId = associatedEventId;
        Exception = ex.ToString();

        AppWindow.Title = "ky3 Launcher Exception Report";

        AppWindowTitleBar titleBar = AppWindow.TitleBar;
        titleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
        titleBar.ExtendsContentIntoTitleBar = true;

        Closed += (_, _) => ProcessFactory.KillCurrent();

        UpdateDragRectangles();
        DraggableGrid.SizeChanged += (_, _) => UpdateDragRectangles();

        SizeInt32 size = new(800, 400);
        AppWindow.Resize(size.Scale(this.RasterizationScale));

        DisplayArea displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);
        if (displayArea is not null)
        {
            RectInt32 workArea = displayArea.WorkArea;
            SizeInt32 windowSize = AppWindow.Size;
            AppWindow.Move(new PointInt32(
                workArea.X + ((workArea.Width - windowSize.Width) / 2),
                workArea.Y + ((workArea.Height - windowSize.Height) / 2)));
        }

        Bindings.Update();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string TraceId { get => $"trace.id: {associatedEventId}"; }

    public string Exception { get; }

    public string? Comment { get; set => SetProperty(ref field, value); }

    public static void Show(CapturedException capturedException)
    {
        Show(capturedException.Id, capturedException.Exception);
    }

    public static void Show(SentryId id, Exception ex)
    {
        ExceptionWindow window = new(id, ex);
        window.AppWindow.Show(true);
        window.AppWindow.MoveInZOrderAtTop();
    }

    [Command("CloseCommand")]
    private async Task CloseWindow()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Close Window", "ExceptionWindow.Command"));

        Bindings.Update();
        if (!string.IsNullOrWhiteSpace(Comment))
        {
            string? email = await Ioc.Default.GetRequiredService<kyxsanUserOptions>().GetActualUserNameAsync().ConfigureAwait(true);
            SentrySdk.CaptureFeedback(Comment, contactEmail: email, associatedEventId: associatedEventId);
        }

        await SentrySdk.FlushAsync().ConfigureAwait(true);
        Close();
    }

    private void UpdateDragRectangles()
    {
        if (!DraggableGrid.IsLoaded)
        {
            return;
        }

        Point position = DraggableGrid.TransformToVisual(Content).TransformPoint(default);
        RectInt32 dragRect = RectInt32Convert.RectInt32(position, DraggableGrid.ActualSize).Scale(this.RasterizationScale);
        InputNonClientPointerSource.GetForWindowId(AppWindow.Id).SetRegionRects(NonClientRegionKind.Caption, [dragRect]);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}