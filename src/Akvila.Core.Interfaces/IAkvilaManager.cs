using AkvilaCore.Interfaces.Integrations;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.Procedures;

namespace AkvilaCore.Interfaces;

public interface IAkvilaManager {
    public ILauncherInfo LauncherInfo { get; }
    public IBugTrackerProcedures BugTracker { get; }
    public IProfileProcedures Profiles { get; }
    public IFileStorageProcedures Files { get; }
    public IServicesIntegrationProcedures Integrations { get; }
    public IUserProcedures Users { get; }
    public ILauncherProcedures Launcher { get; }
    IProfileServersProcedures Servers { get; }
    INotificationProcedures Notifications { get; }
    IModsProcedures Mods { get; }
}
