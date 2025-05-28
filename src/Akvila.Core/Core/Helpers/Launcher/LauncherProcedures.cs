using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Akvila.Core.Constants;
using Akvila.Core.Services.GitHub;
using Akvila.Core.Services.Storage;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.GitHub;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.Procedures;
using AkvilaCore.Interfaces.Storage;

namespace Akvila.Core.Helpers.Launcher;

public class LauncherProcedures : ILauncherProcedures {
    private readonly ILauncherInfo _launcherInfo;
    private readonly IStorageService _storage;
    private readonly IFileStorageProcedures _files;
    private readonly AkvilaManager _akvilaManager;
    private readonly IGitHubService _githubService;
    private ISubject<string> _buildLogs = new Subject<string>();
    private readonly Subject<string> _logsBuffer;
    public IObservable<string> BuildLogs => _buildLogs;
    private const string _launcherGitHub = "https://github.com/AkvilaLauncher/Akvila.Launcher";

    private string[] _allowedVersions = [
        "win-x86",
        "win-x64",
        "win-arm",
        "win-arm64",
        "linux-musl-x64",
        "linux-arm",
        "linux-arm64",
        "linux-x64",
        "osx-x64",
        "osx-arm64",
    ];

    public LauncherProcedures(
        ILauncherInfo launcherInfo,
        IStorageService storage,
        IFileStorageProcedures files,
        AkvilaManager akvilaManager) {
        _logsBuffer = new Subject<string>();

        _logsBuffer
            .Buffer(TimeSpan.FromSeconds(2))
            .Select(items => string.Join(Environment.NewLine, items))
            .Subscribe(combinedText => {
                if (!string.IsNullOrEmpty(combinedText)) {
                    _buildLogs.OnNext(combinedText);
                }
            });

        _akvilaManager = akvilaManager;
        _launcherInfo = launcherInfo;
        _storage = storage;
        _files = files;
        _githubService = new GitHubService(launcherInfo.Settings.HttpClient, akvilaManager);
    }

    public async Task<string> CreateVersion(IVersionFile version, ILauncherBuild launcherBuild) {
        var versions = Directory
            .GetDirectories(launcherBuild.Path)
            .Select(c => new DirectoryInfo(c));

        foreach (var versionInfo in versions) {
            var localVersion = version.Clone() as IVersionFile;

            var splitVersionInfo = versionInfo.Name.Split('-');
            var osName = splitVersionInfo.First();
            var osArch = splitVersionInfo.Last();

            var executeFile = versionInfo.GetFiles("*.*")
                .FirstOrDefault(file => !file.Extension.Equals(".pdb", StringComparison.OrdinalIgnoreCase));

            if (executeFile != null) {
                localVersion!.Guid = await _files.LoadFile(File.OpenRead(executeFile.FullName),
                    Path.Combine("launcher", osName, osArch), $"{versionInfo.Name}-{executeFile.Name}");
            }

            _launcherInfo.ActualLauncherVersion[versionInfo.Name] = localVersion;
            await _storage.SetAsync(StorageConstants.ActualVersion, version.Version);
            await _storage.SetAsync(StorageConstants.ActualVersionInfo, _launcherInfo.ActualLauncherVersion);
        }

        Console.WriteLine();

        // if (version.File is null)
        // {
        //     throw new ArgumentNullException(nameof(version.File));
        // }
        //
        // version.Guid = await _files.LoadFile(version.File, "launcher");
        //
        // _launcherInfo.ActualLauncherVersion[osTypeEnum] = version;
        //
        // await _storage.SetAsync(StorageConstants.ActualVersion, version.Guid);
        // await _storage.SetAsync(StorageConstants.ActualVersionInfo, _launcherInfo.ActualLauncherVersion);
        //
        // await version.File.DisposeAsync();
        // version.File = null;

        return version.Guid;
    }

    public async Task<bool> Build(string version, string[] osNameVersions) {
        var projectPath = new DirectoryInfo(Path.Combine(_launcherInfo.InstallationDirectory, "Launcher", version))
            .FullName;
        var launcherDirectory = new DirectoryInfo(Path.Combine(projectPath, "src", "Akvila.Launcher"));

        if (!Directory.Exists(projectPath)) {
            throw new DirectoryNotFoundException("No sources to generate binary files!");
        }

        var buildFolder = await CreateBuilds(osNameVersions, projectPath, launcherDirectory);

        return buildFolder.IsSuccess;
    }

    public bool CanCompile(string version, out string message) {
        var versionDirectory = Path.Combine(_launcherInfo.InstallationDirectory, "Launcher", version);

        if (!Directory.Exists(versionDirectory)) {
            message =
                $"The profile build for version \"{version}\" is not uploaded, upload it to the server in the folder: \"{Path.Combine(_launcherInfo.InstallationDirectory, "Launcher", version)}\"";
            return false;
        }

        var projectPath = new DirectoryInfo(Path.Combine(_launcherInfo.InstallationDirectory, "Launcher", version))
            .FullName;

        if (string.IsNullOrEmpty(projectPath)) {
            message = $"Couldn't find the project on the path: {projectPath}";
            return false;
        }

        var projectDirectory = new DirectoryInfo(projectPath);

        var projects = projectDirectory.GetFiles("*.csproj", SearchOption.AllDirectories);

        if (!projects.Any(c => c.Name.StartsWith("Akvila.Client"))) {
            message =
                $"Could not find the project on the path: Akvila.Client. Make sure that the project is fully loaded on the server. " +
                "Detailed instructions are available at wiki.recloud.tech: \n" + //TODO: Replace with Akvila wiki link
                "Client part / Building the Launcher / Building from the panel / Uploading source files / Item 2. Uploading";
            return false;
        }

        if (!projects.Any(c => c.Name.StartsWith("GamerVII.Notification.Avalonia"))) {
            message =
                $"Could not find the project on the path: Akvila.Client. Make sure that the project is fully loaded on the server. " +
                "Detailed instructions are available at wiki.recloud.tech: \n" + //TODO: Replace with Akvila wiki link
                "Client part / Building the Launcher / Building from the panel / Uploading source files / Item 2. Uploading";
            return false;
        }

        message = "Success";
        return true;
    }

    public Task<IEnumerable<string>> GetPlatforms() {
        return Task.FromResult<IEnumerable<string>>(_allowedVersions);
    }

    public async Task Download(string version, string host, string folderName) {
        try {
            _buildLogs.OnNext("Preparing launcher downloading...");
            var projectPath = Path.Combine(_akvilaManager.LauncherInfo.InstallationDirectory, "Launcher", version);

            if (Directory.Exists(projectPath)) {
                await _akvilaManager.Notifications
                    .SendMessage("Launcher already exists in the folder, delete it before building it", NotificationType.Error);
                return;
            }

            projectPath = Path.Combine(_akvilaManager.LauncherInfo.InstallationDirectory, "Launcher");

            var allowedVersions = await _githubService
                .GetRepositoryTags("AkvilaLauncher", "Akvila.Launcher");

            if (allowedVersions.All(c => c != version)) {
                await _akvilaManager.Notifications
                    .SendMessage($"The received version of the launcher \"{version}\" is unsupported", NotificationType.Error);
                return;
            }

            _buildLogs.OnNext("Start downloading...");
            var logsDispose = _githubService.Logs.Subscribe(log => _buildLogs.OnNext(log));
            var newFolder = await _githubService.DownloadProject(projectPath, version, _launcherGitHub);
            logsDispose.Dispose();

            _buildLogs.OnNext("Edit configs...");
            await _githubService.EditLauncherFiles(newFolder, host, folderName);
        }
        catch (Exception exception) {
            Console.WriteLine(exception);
            await _akvilaManager.Notifications.SendMessage("Error when loading the Launcher client", exception);
        }
    }

    public Task<IReadOnlyCollection<string>> GetVersions() {
        return _githubService.GetRepositoryTags("AkvilaLauncher", "Akvila.Launcher");
    }


    private async Task<(bool IsSuccess, string Path)> CreateBuilds(string[] versions, string projectPath,
        DirectoryInfo launcherDirectory) {
        var dotnetPath = _launcherInfo.Settings.SystemProcedures.BuildDotnetPath;
        var statusCode = 0;

        foreach (var version in versions.Where(version => _allowedVersions.Contains(version))) {
            var processStartInfo = GetProcessStartInfo(dotnetPath, version, projectPath);
            if (processStartInfo != null) {
                statusCode = await ExecuteProcessAsync(processStartInfo);
            }
        }

        var publishDirectory = launcherDirectory.GetDirectories("publish", SearchOption.AllDirectories);
        var buildsFolder = CreateBuildsFolder();

        foreach (var dir in publishDirectory) {
            var newFolder = new DirectoryInfo(Path.Combine(buildsFolder.FullName, dir.Parent.Name));
            if (!newFolder.Exists) {
                newFolder.Create();
            }

            CopyDirectory(dir, newFolder);
        }

        return (statusCode == 0, buildsFolder.FullName);
    }

    private ProcessStartInfo? GetProcessStartInfo(string dotnetPath, string version, string projectPath) {
        var publishArgs = $"publish ./src/Akvila.Launcher/ -r {version} -c Release -f net8.0 " +
                          "-p:PublishSingleFile=true --self-contained true " +
                          "-p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true " +
                          "-p:PublishReadyToRun=true";

        // var command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        //     ? $"/c {publishArgs}"
        //     : $"-c \"{publishArgs}\"";
        //
        // var fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd" : "/bin/bash";

        return new ProcessStartInfo(dotnetPath, publishArgs) {
            WorkingDirectory = projectPath,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
    }

    private async Task<int> ExecuteProcessAsync(ProcessStartInfo processStartInfo) {
        var process = new Process { StartInfo = processStartInfo };
        process.OutputDataReceived +=
            (sender, e) => _logsBuffer.OnNext($"[{DateTime.Now:HH:mm:ss:fff}] [INFO] {e.Data}");
        process.ErrorDataReceived +=
            (sender, e) => _logsBuffer.OnNext($"[{DateTime.Now:HH:mm:ss:fff}] [ERROR] {e.Data}");

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

#if NETSTANDARD2_1
        process.WaitForExit();
#else
    await process.WaitForExitAsync();
#endif
        return process.ExitCode;
    }

    private DirectoryInfo CreateBuildsFolder() {
        var buildsFolder = new DirectoryInfo(Path.Combine(_launcherInfo.InstallationDirectory, "LauncherBuilds",
            $"build-{DateTime.Now:dd-MM-yyyy HH-mm-ss}"));
        if (!buildsFolder.Exists) {
            buildsFolder.Create();
        }

        return buildsFolder;
    }

    private static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination) {
        if (!destination.Exists) {
            destination.Create();
        }

        foreach (FileInfo file in source.GetFiles()) {
            file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
        }

        foreach (DirectoryInfo subDir in source.GetDirectories()) {
            CopyDirectory(subDir, new DirectoryInfo(Path.Combine(destination.FullName, subDir.Name)));
        }
    }
}
