using AkvilaCore.Interfaces.System;

namespace Akvila.Models.System;

public class LocalFolderInfo : IFolderInfo {
    public LocalFolderInfo() {
    }

    public LocalFolderInfo(string path) {
        Path = path;
    }

    public string Path { get; set; }
}
