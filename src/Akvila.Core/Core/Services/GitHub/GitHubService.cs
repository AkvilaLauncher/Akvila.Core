using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using AkvilaCore.Interfaces;
using AkvilaCore.Interfaces.GitHub;
using Newtonsoft.Json.Linq;

namespace Akvila.Core.Services.GitHub;

public class GitHubService : IGitHubService {
    private readonly IAkvilaManager _akvilaManager;
    private readonly HttpClient _httpClient;
    private IReadOnlyCollection<string> _versions;
    private IReadOnlyCollection<string> _branches;
    private Subject<string> _logs = new();
    public IObservable<string> Logs => _logs;

    public GitHubService(HttpClient httpClient, IAkvilaManager akvilaManager) {
        _akvilaManager = akvilaManager;
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<string>> GetRepositoryBranches(string user, string repository) {
        var url = $"https://api.github.com/repos/{user}/{repository}/branches";

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("request");

        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode) {
            var responseString = await response.Content.ReadAsStringAsync();

            var branches = JArray.Parse(responseString);

            _versions = branches.Select(c => c["name"].ToString()).ToArray();
        }

        return _branches;
    }

    public async Task<IReadOnlyCollection<string>> GetRepositoryTags(string user, string repository) {
        var url = "https://api.github.com/repos/AkvilaLauncher/Akvila.Launcher/tags";

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("request");

        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode) {
            var responseString = await response.Content.ReadAsStringAsync();

            var branches = JArray.Parse(responseString);

            _versions = branches.Select(c => c["name"].ToString()).ToArray();
        }

        return _versions;
    }

    public async Task<string> DownloadProject(string projectPath, string branchName, string repoUrl) {
        var directory = new DirectoryInfo(Path.Combine(projectPath, branchName));

        if (!directory.Exists) {
            directory.Create();
        }

        var processInfo = new ProcessStartInfo {
            FileName = "git",
            Arguments = $"clone --recursive --branch {branchName} {repoUrl} {directory.FullName}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processInfo;
        process.OutputDataReceived += SendLog;
        process.ErrorDataReceived += SendLog;

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();


#if NETSTANDARD2_1
        process.WaitForExit();
#else
        await process.WaitForExitAsync();
#endif

        if (process.ExitCode != 0) {
            throw new InvalidOperationException($"An error occurred while cloning the repository");
        }

        process.OutputDataReceived -= SendLog;
        process.ErrorDataReceived -= SendLog;

        return directory.FullName;
    }

    private void SendLog(object sender, DataReceivedEventArgs e) {
        if (string.IsNullOrEmpty(e.Data)) {
            return;
        }

        _logs.OnNext(e.Data);
    }

    private string NormalizePath(string directory, string fileDirectory) {
        directory = directory
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
        // .TrimStart(Path.DirectorySeparatorChar);

        fileDirectory = fileDirectory
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar)
            .TrimStart(Path.DirectorySeparatorChar);

        return Path.Combine(directory, fileDirectory);
    }

    public async Task EditLauncherFiles(string projectPath, string host, string folder) {
        var keys = new Dictionary<string, string> {
            { "{{HOST}}", host },
            { "{{FOLDER_NAME}}", folder }
        };

        var settingsTemplateFile =
            Path.Combine(projectPath, "src", "Akvila.Launcher", "Assets", "Resources",
                "ResourceKeysDictionary.Template.cs");

        var settingsFile =
            Path.Combine(projectPath, "src", "Akvila.Launcher", "Assets", "Resources", "ResourceKeysDictionary.cs");

        if (File.Exists(settingsFile))
            File.Delete(settingsFile);

        var content = await File.ReadAllTextAsync(settingsTemplateFile);

        foreach (var key in keys) content = content.Replace(key.Key, key.Value);

        await File.WriteAllTextAsync(settingsFile, content);
    }
}
