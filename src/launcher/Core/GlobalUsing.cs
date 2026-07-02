//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

global using CommunityToolkit.Mvvm.DependencyInjection;
global using CommunityToolkit.Mvvm.Messaging;

global using Microsoft;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using Sentry;

global using LauncherRoot = global::Launcher;
global using Launcher.Core.Annotation;
global using Launcher.Core.DependencyInjection;
global using Launcher.Core.DependencyInjection.Annotation;
global using Launcher.Core.Threading;
global using Launcher.Extension;
global using Launcher.Resource.Localization;

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

global using FromKeyed = Launcher.Core.DependencyInjection.Annotation.FromKeyedServicesAttribute;
global using Void = Launcher.Core.Void;

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ky3launcher.Tests")]
