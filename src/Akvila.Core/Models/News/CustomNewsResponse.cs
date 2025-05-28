using System;
using Newtonsoft.Json;

namespace Akvila.Models.News;

public class CustomNewsResponse {
    [JsonProperty("id")] public int Id;
    [JsonProperty("title")] public string? Title;
    [JsonProperty("description")] public string? Description;
    [JsonProperty("createdAt")] public DateTime CreatedAt;
}
