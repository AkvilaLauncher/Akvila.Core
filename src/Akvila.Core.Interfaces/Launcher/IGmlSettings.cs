using System.Net.Http;
using AkvilaCore.Interfaces.Procedures;
using AkvilaCore.Interfaces.Storage;

namespace AkvilaCore.Interfaces.Launcher;

public interface IGmlSettings {
    public string Name { get; }
    public string BaseDirectory { get; }
    public string InstallationDirectory { get; }
    public HttpClient HttpClient { get; }
    IStorageSettings StorageSettings { get; set; }
    string SecurityKey { get; set; }
    ISystemProcedures SystemProcedures { get; }
    string TextureServiceEndpoint { get; set; }
}
