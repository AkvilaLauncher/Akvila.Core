using System.Collections.Generic;
using System.Threading.Tasks;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.Mods;

namespace AkvilaCore.Interfaces.Procedures;

public interface IModsProcedures {
    Task<IEnumerable<IMod>> GetModsAsync(IGameProfile profile);
    Task<IEnumerable<IMod>> GetModsAsync(IGameProfile profile, string name);
    Task<IExternalMod?> GetInfo(string identify, ModType modType);

    Task<IReadOnlyCollection<IModVersion>> GetVersions(IExternalMod modInfo, ModType modType, GameLoader profileLoader,
        string gameVersion);

    Task<IReadOnlyCollection<IExternalMod>> FindModsAsync(GameLoader profileLoader, string gameVersion,
        ModType modLoaderType,
        string modName,
        short take,
        short offset);

    Task SetModDetails(string modName, string title, string description);
    Task Retore();
    ICollection<IModInfo> ModsDetails { get; }
}
