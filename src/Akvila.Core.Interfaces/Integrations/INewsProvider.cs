using System.Collections.Generic;
using System.Threading.Tasks;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.News;

namespace AkvilaCore.Interfaces.Integrations;

public interface INewsProvider {
    void SetManager(IAkvilaManager akvilaManager);
    Task<IReadOnlyCollection<INewsData>> GetNews(int count = 20);
    NewsListenerType Type { get; }
    string Name { get; }
    string Url { get; set; }
}
