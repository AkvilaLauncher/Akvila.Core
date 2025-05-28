using System.IO;
using System.Threading.Tasks;
using AkvilaCore.Interfaces.User;

namespace AkvilaCore.Interfaces.Integrations;

public interface ITextureProvider {
    Task<string> SetSkin(IUser user, string skinUrl);
    Task<string> SetCloak(IUser user, string skinUrl);
    Task<Stream> GetSkinStream(string? textureUrl);
    Task<Stream> GetCloakStream(string? userTextureSkinUrl);
    Task<Stream> GetHeadByNameStream(string? userName);
}
