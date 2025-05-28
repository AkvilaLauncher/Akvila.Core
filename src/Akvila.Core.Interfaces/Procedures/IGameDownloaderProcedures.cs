using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AkvilaCore.Interfaces.Bootstrap;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.System;
using AkvilaCore.Interfaces.User;

namespace AkvilaCore.Interfaces.Procedures;

public interface IGameDownloaderProcedures {
    IObservable<double> FullPercentages { get; }
    IObservable<double> LoadPercentages { get; }
    IObservable<string> LoadLog { get; }
    IObservable<Exception> LoadException { get; }

    Task<string> DownloadGame(string version, string? launchVersion, GameLoader loader,
        IBootstrapProgram? bootstrapProgram);

    Task<Process> CreateProcess(IStartupOptions startupOptions, IUser user, bool needDownload,
        string[] jvmArguments, string[] gameArguments);

    Task<IFileInfo[]> GetAllFiles(bool needRestoreCache);
    Task<IFileInfo[]> GetMods();
    Task<IFileInfo[]> GetOptionalsMods();
    bool GetLauncher(string launcherKey, out object launcher);
    Task<ICollection<IFileInfo>> GetLauncherFiles(string osName, string osArchitecture);
    Task<bool> ValidateProfile(IGameProfile gameProfile);
    Task<FileInfo> AddMod(string fileName, Stream streamData);
    Task<bool> RemoveMod(string fileName);
}
