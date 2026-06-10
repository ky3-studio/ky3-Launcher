//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Text.Json.Annotation;
using System.Collections.Immutable;

namespace kyxsan.Web.Hoyolab.Passport;

internal sealed class VerifyInfo
{
    [JsonPropertyName("status")]
    [JsonEnumHandling(JsonEnumHandling.String)]
    public required VerifyStatus Status { get; set; }

    [JsonPropertyName("verify_method_combinations")]
    public required ImmutableArray<VerifyMethodsWrapper> VerifyMethodCombinations { get; set; }

    [JsonPropertyName("chosen_methods")]
    public required ImmutableArray<int> ChosenMethods { get; set; }

    [JsonPropertyName("partly_verified_methods")]
    public required ImmutableArray<int> PartlyVerifiedMethods { get; set; }
}