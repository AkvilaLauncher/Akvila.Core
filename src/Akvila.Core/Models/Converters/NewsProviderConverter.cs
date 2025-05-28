using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Akvila.Core.Integrations;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.Integrations;

namespace Akvila.Models.Converters;

public class NewsProviderConverter(AkvilaManager akvilaManager) : JsonConverter<INewsProvider> {
    private static readonly Dictionary<NewsListenerType, Type> _typeMapping = new() {
        { NewsListenerType.UnicoreCMS, typeof(UnicoreNewsProvider) },
        { NewsListenerType.Telegram, typeof(TelegramNewsProvider) },
        { NewsListenerType.Azuriom, typeof(AzuriomNewsProvider) },
        { NewsListenerType.VK, typeof(VkNewsProvider) },
        { NewsListenerType.Custom, typeof(CustomNewsProvider) },
    };

    public override void Write(Utf8JsonWriter writer, INewsProvider value, JsonSerializerOptions options) {
        JsonSerializer.Serialize(writer, value, options);
    }

    public override INewsProvider Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        using var document = JsonDocument.ParseValue(ref reader);
        var optionsWithFields = new JsonSerializerOptions(options) {
            IncludeFields = true
        };
        var root = document.RootElement;

        if (!root.TryGetProperty("Type", out var typeProperty))
            throw new JsonException($"Field 'type' not found for deserialization {nameof(INewsProvider)}.");

        if (!Enum.TryParse<NewsListenerType>(typeProperty.ToString(), out var listenerType)) {
            throw new JsonException($"Unknown type {typeProperty} for deserialization {nameof(INewsProvider)}.");
        }

        var targetType = _typeMapping[listenerType];

        var provider = (INewsProvider)JsonSerializer.Deserialize(root.GetRawText(), targetType, optionsWithFields)!;

        provider.SetManager(akvilaManager);

        return provider;
    }

    private static NewsListenerType GetDiscriminatorByType(Type type) {
        foreach (var kvp in _typeMapping) {
            if (kvp.Value == type)
                return kvp.Key;
        }

        throw new JsonException($"Could not find a discriminator for the type {type.FullName}");
    }
}
