//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.DependencyInjection.Abstraction;
using Launcher.Service.Game.FileSystem;
using Launcher.Service.Game.Package.Advanced.AssetOperation;
using Launcher.Service.Game.Package.Advanced.Model;
using Launcher.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using System.IO;

namespace Launcher.Service.Game.Package.Advanced;

internal sealed class GamePackageOperationContext
{
    public GamePackageOperationContext(IServiceProvider serviceProvider, GamePackageOperationKind kind, IGameFileSystem gameFileSystem, string? extractDirectory = default)
    {
        Kind = kind;
        Asset = serviceProvider.GetRequiredService<IDriverMediaTypeAwareFactory<IGameAssetOperation>>().Create(string.Empty);
        GameFileSystem = gameFileSystem;

        EffectiveGameDirectory = extractDirectory ?? gameFileSystem.GameDirectory;

        EffectiveChunksDirectory = kind is GamePackageOperationKind.Verify
            ? Path.Combine(gameFileSystem.ChunksDirectory, "repair")
            : gameFileSystem.ChunksDirectory;
    }

    public GamePackageOperationKind Kind { get; }

    public IGameAssetOperation Asset { get; }

    public IGameFileSystem GameFileSystem { get; init; }

    public SophonDecodedBuild? LocalBuild { get; init; }

    public SophonDecodedBuild? RemoteBuild { get; init; }

    public SophonDecodedPatchBuild? PatchBuild { get; init; }

    public GameChannelSDK? GameChannelSDK { get; init; }

    public string EffectiveGameDirectory { get; }

    public string EffectiveChunksDirectory { get; }
}