using AkvilaCore.Interfaces.Versions;

namespace Akvila.Models.Minecraft;

public class MineVersion : IVersion {
    public string Name { get; set; }
    public bool IsRelease { get; set; }
}
