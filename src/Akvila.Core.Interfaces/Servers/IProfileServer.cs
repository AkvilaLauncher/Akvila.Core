using System.Threading.Tasks;

namespace AkvilaCore.Interfaces.Servers;

public interface IProfileServer {
    public string Name { get; set; }
    public string Address { get; set; }
    public int Port { get; set; }
    Task UpdateStatusAsync();
}
