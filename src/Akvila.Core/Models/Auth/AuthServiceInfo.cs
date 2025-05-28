using AkvilaCore.Interfaces.Auth;
using AkvilaCore.Interfaces.Enums;

namespace Akvila.Models.Auth;

public class AuthServiceInfo : IAuthServiceInfo {
    public AuthServiceInfo(string name, AuthType authType) {
        Name = name;
        AuthType = authType;
    }

    public string Name { get; set; }
    public AuthType AuthType { get; set; }
    public string Endpoint { get; set; }
}
