//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

global using CommunityToolkit.Mvvm.DependencyInjection;
global using CommunityToolkit.Mvvm.Messaging;

global using Microsoft;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using Sentry;

global using kyxsanRoot = global::kyxsan;
global using kyxsan.Core.Annotation;
global using kyxsan.Core.DependencyInjection;
global using kyxsan.Core.DependencyInjection.Annotation;
global using kyxsan.Core.Threading;
global using kyxsan.Extension;
global using kyxsan.Resource.Localization;

global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Windows.Input;

global using FromKeyed = kyxsan.Core.DependencyInjection.Annotation.FromKeyedServicesAttribute;
global using Void = kyxsan.Core.Void;