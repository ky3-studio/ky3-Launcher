//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Hoyolab.HoyoPlay.Connect.Branch;

namespace kyxsan.Web.Endpoint.Hoyolab;

internal static class ApiEndpointsExtension
{
    extension(IApiEndpoints apiEndpoints)
    {
        public string SophonChunkGetBuildByBranch(BranchWrapper wrapper)
        {
            return string.Equals(wrapper.Branch, "PREDOWNLOAD", StringComparison.OrdinalIgnoreCase)
                ? apiEndpoints.SophonChunkGetBuildNoTag(wrapper)
                : apiEndpoints.SophonChunkGetBuild(wrapper);
        }
    }
}