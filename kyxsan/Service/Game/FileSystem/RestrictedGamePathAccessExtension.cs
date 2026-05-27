//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Core.Logging;
using kyxsan.Service.Game.PathAbstraction;

namespace kyxsan.Service.Game.FileSystem;

internal static class RestrictedGamePathAccessExtension
{
    extension(IRestrictedGamePathAccess access)
    {
        public GameFileSystemErrorKind TryGetGameFileSystem(string trace, out IGameFileSystem? fileSystem)
        {
            string? gamePath = access.GamePathEntry.Value?.Path;

            if (string.IsNullOrEmpty(gamePath))
            {
                fileSystem = default;
                SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateDebug($"[{trace}] Error: GamePathNullOrEmpty", "TryGetGameFileSystem"));
                return GameFileSystemErrorKind.GamePathNullOrEmpty;
            }

            if (!access.GamePathLock.TryReaderLock(trace, out AsyncReaderWriterLock.Releaser releaser))
            {
                fileSystem = default;
                SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateDebug($"[{trace}] Error: GamePathLocked", "TryGetGameFileSystem"));
                return GameFileSystemErrorKind.GamePathLocked;
            }

            fileSystem = GameFileSystem.Create(gamePath, releaser);
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateDebug($"[{trace}] Succeed", "TryGetGameFileSystem"));
            return GameFileSystemErrorKind.None;
        }

        public string PerformGamePathEntrySynchronization(string? gamePath = default)
        {
            gamePath ??= access.GamePathEntry.Value?.Path;

            if (string.IsNullOrEmpty(gamePath))
            {
                access.GamePathEntry.Value = default;
                return string.Empty;
            }

            if (access.GamePathEntries.Value.SingleOrDefault(entry => string.Equals(entry.Path, gamePath, StringComparison.OrdinalIgnoreCase)) is { } existed)
            {
                access.GamePathEntry.Value = existed;
                return existed.Path;
            }

            const string LockTrace = $"{nameof(RestrictedGamePathAccessExtension)}.{nameof(PerformGamePathEntrySynchronization)}";
            if (!access.GamePathLock.TryWriterLock(LockTrace, out AsyncReaderWriterLock.Releaser releaser))
            {
                throw kyxsanException.InvalidOperation($"Cannot set game path entries while it is being used. {access.GamePathLock}");
            }

            using (access.GamePathEntry.GetDeferral())
            {
                using (access.GamePathEntries.GetDeferral())
                {
                    using (releaser)
                    {
                        GamePathEntry newEntry = GamePathEntry.Create(gamePath);
                        access.GamePathEntries.Value = access.GamePathEntries.Value.Add(newEntry);
                        access.GamePathEntry.Value = newEntry;
                    }
                }
            }

            return gamePath;
        }

        public string RemoveGamePathEntry(GamePathEntry? entry)
        {
            if (entry is not null)
            {
                const string LockTrace = $"{nameof(RestrictedGamePathAccessExtension)}.{nameof(RemoveGamePathEntry)}";
                if (!access.GamePathLock.TryWriterLock(LockTrace, out AsyncReaderWriterLock.Releaser releaser))
                {
                    throw kyxsanException.InvalidOperation($"Cannot remove game path while it is being used. {access.GamePathLock}");
                }

                using (releaser)
                {
                    // Clear game path if it's selected.
                    if (string.Equals(access.GamePathEntry.Value?.Path, entry.Path, StringComparison.OrdinalIgnoreCase))
                    {
                        access.GamePathEntry.Value = default;
                    }

                    access.GamePathEntries.Value = access.GamePathEntries.Value.Remove(entry);
                }
            }

            return access.PerformGamePathEntrySynchronization();
        }

        public IGameFileSystem UnsafeForceUpdateGamePath(string gamePath, IGameFileSystem old)
        {
            old.Dispose();
            access.PerformGamePathEntrySynchronization(gamePath);
            access.TryGetGameFileSystem($"{nameof(RestrictedGamePathAccessExtension)}.{nameof(UnsafeForceUpdateGamePath)}", out IGameFileSystem? newFileSystem);
            ArgumentNullException.ThrowIfNull(newFileSystem);
            return newFileSystem;
        }
    }


}