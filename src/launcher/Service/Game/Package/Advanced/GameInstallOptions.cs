//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.Abstraction;
using Launcher.Core.ExceptionService;
using Launcher.Service.Game.FileSystem;
using Launcher.Service.Game.Scheme;
using Launcher.Web.Hoyolab.Downloader;

namespace Launcher.Service.Game.Package.Advanced;

internal sealed class GameInstallOptions : IDeconstruct<IGameFileSystem, LaunchScheme>, IDeconstruct<IGameFileSystem, LaunchScheme, SophonBuild>
{
    private readonly IGameFileSystem gameFileSystem;
    private readonly LaunchScheme launchScheme;
    private readonly SophonBuild? betaBuild;

    public GameInstallOptions(IGameFileSystem gameFileSystem, LaunchScheme launchScheme)
    {
        IsBeta = false;
        this.gameFileSystem = gameFileSystem;
        this.launchScheme = launchScheme;

        betaBuild = default!;
    }

    public GameInstallOptions(IGameFileSystem gameFileSystem, LaunchScheme launchScheme, SophonBuild betaBuild)
    {
        IsBeta = true;
        this.gameFileSystem = gameFileSystem;
        this.launchScheme = launchScheme;

        this.betaBuild = betaBuild;
    }

    [MemberNotNullWhen(true, nameof(betaBuild))]
    public bool IsBeta { get; }

    public void Deconstruct(out IGameFileSystem gameFileSystem, out LaunchScheme launchScheme)
    {
        gameFileSystem = this.gameFileSystem;
        launchScheme = this.launchScheme;
    }

    public void Deconstruct(out IGameFileSystem gameFileSystem, out LaunchScheme launchScheme, out SophonBuild betaBuild)
    {
        gameFileSystem = this.gameFileSystem;
        launchScheme = this.launchScheme;
        LauncherException.ThrowIfNot(IsBeta, "Deconstruct to beta build when not a beta install");
        betaBuild = this.betaBuild;
    }
}