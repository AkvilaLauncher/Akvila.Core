using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AkvilaCore.Interfaces;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.Integrations;
using AkvilaCore.Interfaces.News;

namespace Akvila.Core.Integrations;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
public abstract class BaseNewsProvider : INewsProvider {
    public IAkvilaManager AkvilaManager { get; set; }

    public virtual void SetManager(IAkvilaManager akvilaManager) {
        AkvilaManager = akvilaManager;
    }

    public abstract Task<IReadOnlyCollection<INewsData>> GetNews(int count = 20);
    public NewsListenerType Type { get; set; }
    public virtual string Name => GetType().Name;
    public string Url { get; set; }

    public BaseNewsProvider() {
    }

    public override bool Equals(object? obj) {
        if (obj is BaseNewsProvider provider) {
            return provider.Type == Type;
        }

        return false;
    }

    protected bool Equals(BaseNewsProvider other) {
        return Type == other.Type;
    }

    public override int GetHashCode() {
        return (int)Type;
    }
}
