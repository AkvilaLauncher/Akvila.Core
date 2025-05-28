using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.Storage;

namespace Akvila.Models.Storage;

public class StorageSettings : IStorageSettings {
    public StorageType StorageType { get; set; }
    public string StorageHost { get; set; }
    public string StorageLogin { get; set; }
    public string StoragePassword { get; set; }
    public TextureProtocol TextureProtocol { get; set; }
}
