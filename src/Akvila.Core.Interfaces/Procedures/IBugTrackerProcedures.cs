using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.Sentry;

namespace AkvilaCore.Interfaces.Procedures;

public interface IBugTrackerProcedures {
    void CaptureException(IBugInfo bugInfo);
    IBugInfo CaptureException(Exception exception);
    Task<IEnumerable<IBugInfo>> GetAllBugs();
    Task<IBugInfo?> GetBugId(Guid id);
    Task<IEnumerable<IBugInfo>> GetFilteredBugs(Expression<Func<IStorageBug, bool>> filter);
}
