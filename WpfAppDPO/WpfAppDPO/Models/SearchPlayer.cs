using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfAppDPO.Models
{
    public static class SearchPlayer
    {
        public static bool check = true;
        public static async Task SearchWG()
        {

            // Игрок
            using (var client = new HttpClient())
            {
                Variables.players.Clear();
                client.BaseAddress = new Uri("https://api.wotblitz.ru/");

                foreach (var nick in Variables.nicks)
                {
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("application_id", "8c4eecab18df2fd980424d5e35dba7bd"),
                        new KeyValuePair<string, string>("search", $"{nick}"), // Ник игрока которого надо найти
                        new KeyValuePair<string, string>("type", "exact")
                    });

                    Thread.Sleep(500);

                    var result = await client.PostAsync("/wotb/account/list/", content);
                    string json = await result.Content.ReadAsStringAsync();
                    var player = JsonConvert.DeserializeObject<PlayerWG.Player>(json);
                    Variables.players.Add(player);
                }
            }

            // Информация о игроке
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("https://api.wotblitz.ru/");
            //    var content = new FormUrlEncodedContent(new[]
            //    {
            //        new KeyValuePair<string, string>("application_id", "8c4eecab18df2fd980424d5e35dba7bd"),
            //        new KeyValuePair<string, string>("account_id", "71941826"),
            //        new KeyValuePair<string, string>("extra", "statistics.rating")
            //    });
            //    var result = await client.PostAsync("/wotb/account/info/", content);
            //    string json = await result.Content.ReadAsStringAsync();
            //    int startIndex = json.IndexOf(@"""data"":{""") + 9;
            //    int endIndex = json.LastIndexOf(@""":{""statistics""");
            //    var str = json.Remove(startIndex, endIndex - startIndex).Insert(startIndex, "info");
            //    var account = JsonConvert.DeserializeObject<Account>(str);
            //    var accountStartSession = File.Exists("account.json") ? JsonConvert.DeserializeObject<Account>(File.ReadAllText("account.json")) : JsonConvert.DeserializeObject<Account>(str);

            //    while (check)
            //    {
            //        File.WriteAllText("account.json", JsonConvert.SerializeObject(account));
            //        check = false;
            //    }
            //}
        }
    }
}
