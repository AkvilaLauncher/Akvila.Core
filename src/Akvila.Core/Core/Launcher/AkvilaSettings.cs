using System.IO;
using System.Net.Http;
using Akvila.Core.Helpers.System;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.Procedures;
using AkvilaCore.Interfaces.Storage;

namespace Akvila.Core.Launcher;

public class AkvilaSettings : IGmlSettings {
    private readonly ISystemProcedures _systemProcedures;
    public ISystemProcedures SystemProcedures => _systemProcedures;

    public AkvilaSettings(string name, string securityKey, string? baseDirectory = null,
        HttpClient? httpClient = null) {
        HttpClient = httpClient ?? new HttpClient();
        _systemProcedures = new SystemProcedures(this);

        Name = name;
        SecurityKey = securityKey;
        FolderName = _systemProcedures.CleanFolderName(name);
        BaseDirectory = string.IsNullOrEmpty(baseDirectory) ? _systemProcedures.DefaultInstallation : baseDirectory;
        InstallationDirectory = Path.Combine(BaseDirectory, FolderName);
    }

    public string FolderName { get; }
    public string Name { get; }
    public string BaseDirectory { get; }
    public string InstallationDirectory { get; }
    public HttpClient HttpClient { get; }

    public IStorageSettings StorageSettings { get; set; }
    public string SecurityKey { get; set; }
    public string TextureServiceEndpoint { get; set; }
}
