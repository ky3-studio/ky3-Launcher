//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.IO;
using kyxsan.Factory.Picker;

namespace kyxsan.Service.Game.Locator;

[Service(ServiceLifetime.Transient, typeof(IGameLocator), Key = GameLocationSourceKind.Manual)]
internal sealed partial class ManualGameLocator : IGameLocator
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;

    [GeneratedConstructor]
    public partial ManualGameLocator(IServiceProvider serviceProvider);

    public ValueTask<ValueResult<bool, string>> LocateSingleGamePathAsync()
    {
        (bool isPickerOk, ValueFile file) = fileSystemPickerInteraction.PickFile(
            SH.ServiceGameLocatorFileOpenPickerCommitText,
            SH.ServiceGameLocatorPickerFilterText,
            $"{GameConstants.YuanShenFileName};{GameConstants.GenshinImpactFileName}");

        if (isPickerOk)
        {
            string fileName = System.IO.Path.GetFileName(file);
            if (fileName.ToUpperInvariant() is GameConstants.YuanShenFileNameUpper or GameConstants.GenshinImpactFileNameUpper)
            {
                return ValueTask.FromResult<ValueResult<bool, string>>(new(true, file));
            }
        }

        return ValueTask.FromResult<ValueResult<bool, string>>(new(false, default!));
    }
}