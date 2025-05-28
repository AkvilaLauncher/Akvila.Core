using AkvilaCore.Interfaces.Enums;

namespace AkvilaCore.Interfaces.Auth;

public interface IAuthServiceInfo {
    public string Name { get; set; }
    public AuthType AuthType { get; set; }
    string Endpoint { get; set; }
}
