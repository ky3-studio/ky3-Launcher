//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Launcher.Core.Database;
using Launcher.Model.Entity.Database;
using Launcher.Service.Game.AdvancedStart.Model;
using System.Collections.ObjectModel;

namespace Launcher.Service.Game.AdvancedStart;

[Service(ServiceLifetime.Singleton)]
internal sealed class AdvancedStartDelayedProgramStore
{
    private readonly IServiceProvider serviceProvider;

    public AdvancedStartDelayedProgramStore(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public ObservableCollection<AdvancedStartDelayedProgramEntry> Load()
    {
        AdvancedStartDelayedProgramsConfiguration config = GetConfiguration();

        List<AdvancedStartDelayedProgramEntry> list = config.Entries
            .Select(static e => new AdvancedStartDelayedProgramEntry(e.Name, e.Path, unchecked((int)e.DelaySeconds)))
            .ToList();

        return new(list);
    }

    public void Save(ObservableCollection<AdvancedStartDelayedProgramEntry> entries)
    {
        AdvancedStartDelayedProgramsConfiguration config = new();
        config.Entries.AddRange(entries.Select(static e => new AdvancedStartDelayedProgramEntryDto
        {
            Name = e.Name ?? string.Empty,
            Path = e.Path ?? string.Empty,
            DelaySeconds = unchecked((uint)Math.Max(0, e.DelaySeconds)),
        }));

        SetConfiguration(config);
    }

    private AdvancedStartDelayedProgramsConfiguration GetConfiguration()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            string? base64 = db.Settings.SingleOrDefault(e => e.Key == Core.Setting.SettingKeys.LaunchAdvancedStartDelayedPrograms)?.Value;
            if (string.IsNullOrEmpty(base64))
            {
                return new();
            }

            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                AdvancedStartDelayedProgramsConfiguration config = new();
                config.MergeFrom(bytes);
                return config;
            }
            catch
            {
                return new();
            }
        }
    }

    private void SetConfiguration(AdvancedStartDelayedProgramsConfiguration config)
    {
        string base64 = Convert.ToBase64String(config.ToByteArray());

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Settings.Where(e => e.Key == Core.Setting.SettingKeys.LaunchAdvancedStartDelayedPrograms).ExecuteDelete();
            db.Settings.AddAndSave(new(Core.Setting.SettingKeys.LaunchAdvancedStartDelayedPrograms, base64));
        }
    }
}