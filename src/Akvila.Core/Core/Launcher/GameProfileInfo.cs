using System.Collections.Generic;
using Akvila.Models.System;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.Launcher;

namespace Akvila.Core.Launcher;

public class GameProfileInfo : IGameProfileInfo {
    public string JavaPath { get; set; }
    public string IconBase64 { get; set; }
    public string Description { get; set; }
    public ProfileState State { get; set; }
    public IEnumerable<LocalFileInfo> Files { get; set; }
    public IEnumerable<LocalFolderInfo> WhiteListFolders { get; set; }
    public IEnumerable<LocalFileInfo> WhiteListFiles { get; set; }
    public bool HasUpdate { get; set; }
    public string ProfileName { get; set; }
    public string DisplayName { get; set; }
    public string MinecraftVersion { get; set; }
    public string ClientVersion { get; set; }
    public string LaunchVersion { get; set; }
    public string Arguments { get; set; }
    public string JvmArguments { get; set; }
    public string GameArguments { get; set; }
}
