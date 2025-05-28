using System.Threading.Tasks;
using AkvilaCore.Interfaces.Launcher;

namespace AkvilaCore.Interfaces.Plugins;

public interface IPlugin {
    public Task Execute(ILauncherInfo launcherInfo);
}
