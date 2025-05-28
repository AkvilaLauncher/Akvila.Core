using System.Collections.Generic;
using System.Threading.Tasks;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.News;

namespace Akvila.Core.Integrations;

public class TelegramNewsProvider : BaseNewsProvider {
    public NewsListenerType Type { get; }

    public override Task<IReadOnlyCollection<INewsData>> GetNews(int count = 20) {
        throw new System.NotImplementedException();
    }
}
