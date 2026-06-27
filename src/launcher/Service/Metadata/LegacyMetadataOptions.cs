//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core;
using Launcher.Web.Endpoint.Launcher;
using System.IO;

namespace Launcher.Service.Metadata;

// This file is left here for future compatibility issues.
// [Service(ServiceLifetime.Singleton)]
internal sealed partial class LegacyMetadataOptions
{
    private readonly ILauncherEndpointsFactory LauncherEndpointsFactory;
    private readonly CultureOptions cultureOptions;

    [GeneratedConstructor]
    public partial LegacyMetadataOptions(IServiceProvider serviceProvider);

    [field: MaybeNull]
    public string LocalizedDataFolder
    {
        get
        {
            if (field is null)
            {
                field = Path.Combine(LauncherRuntime.DataDirectory, "Metadata", cultureOptions.LocaleName);
                Directory.CreateDirectory(field);
            }

            return field;
        }
    }

    public string GetTemplateEndpoint()
    {
        return LauncherEndpointsFactory.Create().MetadataTemplate();
    }

    public string GetLocalizedLocalPath(string fileNameWithExtension)
    {
        return Path.Combine(LocalizedDataFolder, fileNameWithExtension);
    }

    public string GetLocalizedRemoteFile(MetadataTemplate? templateInfo, string fileNameWithExtension)
    {
        return templateInfo is { Template: { } template }
            ? LauncherEndpointsFactory.Create().Metadata(template, cultureOptions.LocaleName, fileNameWithExtension)
            : LauncherEndpointsFactory.Create().Metadata(cultureOptions.LocaleName, fileNameWithExtension);
    }
}