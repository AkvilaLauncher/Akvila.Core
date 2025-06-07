using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Akvila.Models.Launcher;
using Akvila.Models.Storage;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.Storage;

namespace Akvila.Core.Launcher;

public class AccessTokenTokens {
    public const string CurseForgeKey = "CurseForgeKey";
    public const string VkKey = "VkKey";
}

public class LauncherInfo : ILauncherInfo {
    private readonly IAkvilaSettings _settings;
    private Subject<IStorageSettings> _settingsUpdated = new();

    public string Name => _settings.Name;
    public string BaseDirectory => _settings.BaseDirectory;
    public string InstallationDirectory => _settings.InstallationDirectory;
    public IAkvilaSettings Settings => _settings;
    public IStorageSettings StorageSettings { get; set; } = new StorageSettings();
    public IObservable<IStorageSettings> SettingsUpdated => _settingsUpdated;
    public IDictionary<string, string> AccessTokens { get; set; } = new ConcurrentDictionary<string, string>();
    public Dictionary<string, IVersionFile?> ActualLauncherVersion { get; set; } = new();

    public LauncherInfo(IAkvilaSettings settings) {
        _settings = settings;
    }

    public void UpdateSettings(StorageType storageType,
        string storageHost,
        string storageLogin,
        string storagePassword,
        TextureProtocol textureProtocol,
        string curseForgeKey,
        string vkKey) {
        StorageSettings.StoragePassword = storagePassword;
        StorageSettings.StorageLogin = storageLogin;
        StorageSettings.StorageType = storageType;
        StorageSettings.StorageHost = storageHost;
        StorageSettings.TextureProtocol = textureProtocol;
        AccessTokens[AccessTokenTokens.CurseForgeKey] = curseForgeKey;
        AccessTokens[AccessTokenTokens.VkKey] = vkKey;

        _settingsUpdated.OnNext(StorageSettings);
    }

    public Task<IEnumerable<ILauncherBuild>> GetBuilds() {
        var versionsPath = Path.Combine(InstallationDirectory, "LauncherBuilds");

        var directoryInfo = new DirectoryInfo(versionsPath);

        var builds = directoryInfo.GetDirectories();

        var versions = builds.Select(c => new LauncherBuild {
            Name = c.Name,
            Path = c.FullName,
            DateTime = c.CreationTime
        });

        return Task.FromResult(versions.OfType<ILauncherBuild>());
    }

    public async Task<ILauncherBuild?> GetBuild(string name) {
        return (await GetBuilds()).FirstOrDefault(c => c.Name == name);
    }
}
