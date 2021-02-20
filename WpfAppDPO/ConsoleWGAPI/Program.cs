using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.IO;

namespace ConsoleWGAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "WG API TEST";
            while (true)
            {
                Task.Run(() => MainAsync());
                Thread.Sleep(10000);
                Console.Clear();
            }

        }

        static async Task MainAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.wotblitz.ru/");
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("application_id", "8c4eecab18df2fd980424d5e35dba7bd"),
                new KeyValuePair<string, string>("account_id", "71941826"),
                new KeyValuePair<string, string>("extra", "statistics.rating")
            });
                var result = await client.PostAsync("/wotb/account/info/", content);
                string json = await result.Content.ReadAsStringAsync();
                int startIndex = json.IndexOf(@"""data"":{""") + 9;
                int endIndex = json.LastIndexOf(@""":{""statistics""");
                var str = json.Remove(startIndex, endIndex - startIndex).Insert(startIndex, "info");
                var account = JsonConvert.DeserializeObject<Account>(str);
                var accountStartSession = File.Exists("account.json") ? JsonConvert.DeserializeObject<Account>(File.ReadAllText("account.json")) : JsonConvert.DeserializeObject<Account>(str);

                while (Variables.saveJSON)
                {
                    File.WriteAllText("account.json", JsonConvert.SerializeObject(account));
                    Variables.saveJSON = false;
                }

                float wins = (float)account.data.info.statistics.all.wins - accountStartSession.data.info.statistics.all.wins;
                float battles = (float)account.data.info.statistics.all.battles - accountStartSession.data.info.statistics.all.battles;
                int frags = account.data.info.statistics.all.frags - accountStartSession.data.info.statistics.all.frags;
                int survived_battles = account.data.info.statistics.all.survived_battles - accountStartSession.data.info.statistics.all.survived_battles;
                float fragsPercent = (float)frags / battles;
                float deaths = (float)battles - survived_battles;
                float winRate = (float)(wins) / (battles) * 100;

                Console.WriteLine($"Session\n" +
                    $"Wins/Battles:\t{wins} ({battles})\t({Math.Round(winRate, 2)}%)\n" +
                    $"Kills:\t\t{frags}\t({fragsPercent})\n" +
                    $"Deaths:\t\t{deaths}\t({deaths / battles * 100}%)\n" +
                    $"Hits/Shots:\t\n" +
                    $"Penetrations:\t\n" +
                    $"Damage Dealt:\t\n" +
                    $"Damage Received:\t\n" +
                    $"Spotted:\t\n" +
                    $"Defence:\t\n" +
                    $"Capture:\t\n\n" +
                    $"С/У:\t{Math.Round((float)(account.data.info.statistics.all.damage_dealt - accountStartSession.data.info.statistics.all.damage_dealt) / (account.data.info.statistics.all.battles - accountStartSession.data.info.statistics.all.battles), 0)}\n");
            }
        }
    }
}
