//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Web.Endpoint.kyxsan;
using System.IO;

namespace kyxsan.Service.Metadata;

// This file is left here for future compatibility issues.
// [Service(ServiceLifetime.Singleton)]
internal sealed partial class LegacyMetadataOptions
{
    private readonly IkyxsanEndpointsFactory kyxsanEndpointsFactory;
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
                field = Path.Combine(kyxsanRuntime.DataDirectory, "Metadata", cultureOptions.LocaleName);
                Directory.CreateDirectory(field);
            }

            return field;
        }
    }

    public string GetTemplateEndpoint()
    {
        return kyxsanEndpointsFactory.Create().MetadataTemplate();
    }

    public string GetLocalizedLocalPath(string fileNameWithExtension)
    {
        return Path.Combine(LocalizedDataFolder, fileNameWithExtension);
    }

    public string GetLocalizedRemoteFile(MetadataTemplate? templateInfo, string fileNameWithExtension)
    {
        return templateInfo is { Template: { } template }
            ? kyxsanEndpointsFactory.Create().Metadata(template, cultureOptions.LocaleName, fileNameWithExtension)
            : kyxsanEndpointsFactory.Create().Metadata(cultureOptions.LocaleName, fileNameWithExtension);
    }
}