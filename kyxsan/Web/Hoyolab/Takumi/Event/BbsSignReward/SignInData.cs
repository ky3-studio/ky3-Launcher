//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Endpoint.Hoyolab;

namespace kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;

internal sealed class SignInData
{
    public SignInData(IApiEndpoints apiEndpoints, PlayerUid uid)
    {
        ActivityId = apiEndpoints.LunaSolActivityId();
        Region = uid.Region;
        Uid = uid.Value;
    }

    [JsonPropertyName("act_id")]
    public string ActivityId { get; }

    [JsonPropertyName("region")]
    public Region Region { get; }

    [JsonPropertyName("uid")]
    public string Uid { get; }
}