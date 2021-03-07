using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Не актуально, удалить
namespace WpfAppDPO.Models
{
    public static class ScrapDataPlayer
    {
        public static async Task DataPlayer()
        {
            // Информация о игроке
            using (var client = new HttpClient())
            {
                Variables.accounts.Clear();
                client.BaseAddress = new Uri("https://api.wotblitz.ru/");

                for (int i = 0; i < Variables.nicks.Count; i++)
                {
                    Thread.Sleep(500);

                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("application_id", "8c4eecab18df2fd980424d5e35dba7bd"),
                        new KeyValuePair<string, string>("account_id", $"{Variables.players.ElementAt(i).data.ElementAt(0).account_id}"),
                        new KeyValuePair<string, string>("extra", "statistics.rating")
                    });

                    var result = await client.PostAsync("/wotb/account/info/", content);
                    string json = await result.Content.ReadAsStringAsync();
                    int startIndex = json.IndexOf(@"""data"":{""") + 9;
                    int endIndex = json.LastIndexOf(@""":{""statistics""");
                    var str = json.Remove(startIndex, endIndex - startIndex).Insert(startIndex, "info");
                    var account = JsonConvert.DeserializeObject<Account>(str);
                    Variables.accounts.Add(account);
                }
            }
        }
    }
}
