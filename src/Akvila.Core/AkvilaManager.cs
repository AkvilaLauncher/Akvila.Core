using System;
using System.Collections.Generic;
using Akvila.Core.Constants;
using Akvila.Core.Helpers.BugTracker;
using Akvila.Core.Helpers.Files;
using Akvila.Core.Helpers.Launcher;
using Akvila.Core.Helpers.Mods;
using Akvila.Core.Helpers.Notifications;
using Akvila.Core.Helpers.Profiles;
using Akvila.Core.Helpers.User;
using Akvila.Core.Integrations;
using Akvila.Core.Launcher;
using Akvila.Core.Services.Storage;
using AkvilaCore.Interfaces;
using AkvilaCore.Interfaces.Integrations;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.Procedures;
using AkvilaCore.Interfaces.Storage;

namespace Akvila;

public class AkvilaManager : IAkvilaManager {
    private readonly IGmlSettings _settings;

    public AkvilaManager(IGmlSettings settings) {
        _settings = settings;
        LauncherInfo = new LauncherInfo(settings);
        Storage = new SqliteStorageService(settings);
        BugTracker = new BugTrackerProcedures(Storage, settings);
        Notifications = new NotificationProcedures(Storage);
        Profiles = new ProfileProcedures(LauncherInfo, Storage, Notifications, BugTracker, this);
        Files = new FileStorageProcedures(LauncherInfo, Storage, BugTracker);
        Mods = new ModsProcedures(LauncherInfo, settings, Storage, BugTracker);
        Integrations = new ServicesIntegrationProcedures(settings, Storage, BugTracker, this);
        Users = new UserProcedures(settings, Storage, this);
        Launcher = new LauncherProcedures(LauncherInfo, Storage, Files, this);
        Servers = (IProfileServersProcedures)Profiles;
    }

    public IStorageService Storage { get; }
    public ILauncherInfo LauncherInfo { get; }
    public IBugTrackerProcedures BugTracker { get; }
    public IProfileProcedures Profiles { get; }
    public IProfileServersProcedures Servers { get; }
    public IFileStorageProcedures Files { get; }
    public IServicesIntegrationProcedures Integrations { get; }
    public IUserProcedures Users { get; }
    public IModsProcedures Mods { get; }
    public ILauncherProcedures Launcher { get; }
    public ISystemProcedures System => _settings.SystemProcedures;
    public INotificationProcedures Notifications { get; }

    public void RestoreSettings<T>() where T : IVersionFile {
        try {
            Profiles.RestoreProfiles().Wait();
            Notifications.Retore().Wait();
            Integrations.NewsProvider.Restore().Wait();
            Mods.Retore().Wait();

            var versionReleases =
                Storage.GetAsync<Dictionary<string, T?>>(StorageConstants.ActualVersionInfo).Result;

            if (versionReleases is null) return;

            foreach (var item in versionReleases) {
                LauncherInfo.ActualLauncherVersion.Add(item.Key, item.Value);
            }
        }
        catch (Exception exception) {
            BugTracker.CaptureException(exception);
            Console.WriteLine(exception);
        }
    }
}
