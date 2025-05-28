using System;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.Notifications;

namespace Akvila.Core.Helpers.Notifications;

public record Notification : INotification {
    public string Message { get; set; }
    public string Details { get; set; }
    public NotificationType Type { get; set; }
    public DateTimeOffset Date { get; set; }
}
