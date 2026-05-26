//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.IO;

namespace kyxsan.Factory.Picker;

internal static class FileSystemPickerInteractionExtension
{
    extension(IFileSystemPickerInteraction interaction)
    {
        public ValueResult<bool, ValueFile> PickFile(FileSystemPickerOptions options)
        {
            return interaction.PickFile(options.Title, options.DefaultFileName, options.FilterName, options.FilterType);
        }

        public ValueResult<bool, ValueFile> SaveFile(FileSystemPickerOptions options)
        {
            return interaction.SaveFile(options.Title, options.DefaultFileName, options.FilterName, options.FilterType);
        }

        public ValueResult<bool, ValueFile> PickFile(string? title, string? filterName, string? filterType)
        {
            return interaction.PickFile(title, null, filterName, filterType);
        }

        public ValueResult<bool, string?> PickFolder()
        {
            return interaction.PickFolder(null);
        }
    }
}