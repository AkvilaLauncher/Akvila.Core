using System;
using AkvilaCore.Interfaces.User;

namespace Akvila.Models.Sessions;

public class GameSession : ISession {
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public GameSession() {
        Start = DateTimeOffset.Now;
    }
}
