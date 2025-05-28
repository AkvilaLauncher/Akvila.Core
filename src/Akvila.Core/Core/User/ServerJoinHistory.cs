using System;

namespace Akvila.Core.User;

public class ServerJoinHistory(string serverUuid, DateTime date) {
    public string ServerUuid { get; } = serverUuid;
    public DateTime Date { get; } = date;
}
