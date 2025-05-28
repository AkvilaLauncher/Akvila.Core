using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.Storage;

namespace AkvilaCore.Interfaces.Procedures;

public interface ILauncherProcedures {
    Task<string> CreateVersion(IVersionFile version, ILauncherBuild launcherBuild);
    Task<bool> Build(string version, string[] osNameVersions);
    IObservable<string> BuildLogs { get; }
    bool CanCompile(string version, out string message);
    Task<IEnumerable<string>> GetPlatforms();
    Task Download(string version, string host, string folderName);
    Task<IReadOnlyCollection<string>> GetVersions();
}
