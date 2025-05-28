namespace AkvilaCore.Interfaces.Bootstrap;

public interface IBootstrapProgram {
    string Name { get; set; }
    string Version { get; set; }
    int MajorVersion { get; set; }
}
