using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using System.Threading;

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
                float WinRate = (float)account.data.info.statistics.all.wins / account.data.info.statistics.all.battles * 100;

                Console.WriteLine($"Ник: {account.data.info.nickname}\nПобеды: {Math.Round(WinRate, 2)}%\nС/У: {Math.Round((float)account.data.info.statistics.all.damage_dealt / account.data.info.statistics.all.battles, 0)}");
            }
        }
    }
}
