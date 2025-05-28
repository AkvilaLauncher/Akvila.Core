using System;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.News;

namespace Akvila.Models.News;

public class NewsData : INewsData {
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTimeOffset Date { get; set; }
    public NewsListenerType Type { get; set; }
}
