using AkvilaCore.Interfaces.Enums;

namespace AkvilaCore.Interfaces.News;

public class INews {
    public string Url { get; set; }
    public NewsListenerType Type { get; set; }
}
