using System.Threading.Tasks;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.Servers;

namespace AkvilaCore.Interfaces.Procedures;

public interface IProfileServersProcedures {
    Task<IProfileServer> AddMinecraftServer(IGameProfile profileprofile, string serverName, string address, int port);
    Task UpdateServerState(IProfileServer minecraftServer);
    Task RemoveServer(IGameProfile profile, string serverName);
}
