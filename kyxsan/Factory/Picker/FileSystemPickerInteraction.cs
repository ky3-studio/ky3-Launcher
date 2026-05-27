//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.IO;
using kyxsan.Core.LifeCycle;

namespace kyxsan.Factory.Picker;

[Service(ServiceLifetime.Transient, typeof(IFileSystemPickerInteraction))]
internal sealed partial class FileSystemPickerInteraction : IFileSystemPickerInteraction
{
    private readonly ICurrentXamlWindowReference currentWindowReference;

    [GeneratedConstructor]
    public partial FileSystemPickerInteraction(IServiceProvider serviceProvider);

    public ValueResult<bool, ValueFile> PickFile(string? title, string? defaultFileName, string? filterName, string? filterType)
    {
        bool picked = FileSystem.PickFile(currentWindowReference.WindowHandle, title, defaultFileName, filterName, filterType, out string? path);
        return new(picked, path);
    }

    public ValueResult<bool, ValueFile> SaveFile(string? title, string? defaultFileName, string? filterName, string? filterType)
    {
        bool picked = FileSystem.SaveFile(currentWindowReference.WindowHandle, title, defaultFileName, filterName, filterType, out string? path);
        return new(picked, path);
    }

    public ValueResult<bool, string?> PickFolder(string? title)
    {
        bool picked = FileSystem.PickFolder(currentWindowReference.WindowHandle, title, out string? path);
        return new(picked, path);
    }
}