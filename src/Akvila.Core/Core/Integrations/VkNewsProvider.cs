using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Akvila.Core.Launcher;
using Akvila.Models.News;
using AkvilaCore.Interfaces;
using AkvilaCore.Interfaces.Enums;
using AkvilaCore.Interfaces.Launcher;
using AkvilaCore.Interfaces.News;

namespace Akvila.Core.Integrations;

public class VkNewsProvider : BaseNewsProvider {
    public override string Name => "Вконтакте";

    private readonly IAkvilaManager _akvilaManager;
    private string _accessToken;

    public VkNewsProvider() {
    }

    public VkNewsProvider(string groupId, IAkvilaManager akvilaManager) {
        Type = NewsListenerType.VK;
        _akvilaManager = akvilaManager;
        Url = groupId.Split(".com/").Last();

        SaveToken(akvilaManager.LauncherInfo);

        akvilaManager.LauncherInfo.SettingsUpdated.Subscribe(_ => SaveToken(akvilaManager.LauncherInfo));
    }

    private void SaveToken(ILauncherInfo launcherInfo) {
        if (launcherInfo.AccessTokens.TryGetValue(AccessTokenTokens.VkKey, out var token) &&
            !string.IsNullOrEmpty(token)) {
            _accessToken = token;
        }

        _ = GetNews();
    }

    public override async Task<IReadOnlyCollection<INewsData>> GetNews(int count = 20) {
        try {
            if (string.IsNullOrEmpty(_accessToken)) {
                return [];
            }

            var url = "https://api.vk.com/method/wall.get";
            using var client = new HttpClient();

            var parameters = new Dictionary<string, string> {
                { "domain", Url },
                { "count", count.ToString() },
                { "access_token", _accessToken },
                { "v", "5.131" }
            };

            var content = new FormUrlEncodedContent(parameters);

            var response = await client.GetAsync(url + "?" + await content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode) {
                var json = await response.Content.ReadAsStringAsync();

                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<VkNewsResponse>(json);

                if (data is null)
                    return Array.Empty<INewsData>();

                return data.Response?.Items.Select(x => new NewsData {
                    Title = x.Title ?? "Нет заголовка",
                    Content = x.Text,
                    Type = NewsListenerType.VK,
                    Date = DateTimeOffset.FromUnixTimeSeconds(x.Date),
                }).ToList() ?? [];
            }

            return [];
        }
        catch (Exception e) {
            _akvilaManager.BugTracker.CaptureException(e);

            return [];
        }
    }

    public override void SetManager(IAkvilaManager akvilaManager) {
        base.SetManager(akvilaManager);

        SaveToken(akvilaManager.LauncherInfo);

        akvilaManager.LauncherInfo.SettingsUpdated.Subscribe(_ => SaveToken(akvilaManager.LauncherInfo));
    }
}
