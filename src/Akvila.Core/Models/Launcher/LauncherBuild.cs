using System;
using AkvilaCore.Interfaces.Launcher;

namespace Akvila.Models.Launcher;

public class LauncherBuild : ILauncherBuild {
    public string Name { get; set; }
    public string Path { get; set; } = null!;
    public string ExecutableFilePath { get; set; } = null!;
    public DateTime DateTime { get; set; }
}
