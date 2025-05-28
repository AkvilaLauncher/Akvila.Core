using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AkvilaCore.Interfaces.Procedures;
using AkvilaCore.Interfaces.Servers;

namespace Akvila.Models.Servers;

public class MinecraftServer : IProfileServer {
    [JsonIgnore] public IProfileServersProcedures ServerProcedures { get; set; }

    public string Name { get; set; }
    public string Address { get; set; }
    public int Port { get; set; }
    public string Version { get; set; }
    public bool IsOnline { get; set; }
    public int? Online { get; set; }
    public int? MaxOnline { get; set; }

    public Task UpdateStatusAsync() {
        try {
            return ServerProcedures.UpdateServerState(this);
        }
        catch (Exception e) {
            Console.WriteLine(e);
        }

        return Task.CompletedTask;
    }
}
