using BotTest2.Models;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotTest2
{
    class Program
    {
        public DbContext Db { get; set; }
        public IList<User> Users { get; set; }
        public Timer _timer { get; set; }
        public DiscordSocketClient client { get; set; }

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.Log += Log;
            client.MessageReceived += MessageReceived;
            client.Ready += OnReady;
            Db = new DbContext();
            Users = Db.Users;
            string token = "Mzk5ODc3OTI5NDM5MzMwMzA2.DX5_cQ.lDQUWxs6hs-jcY2LiCibsG54azw";
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();


            // Block this task until the program is closed.
            await Task.Delay(-1);

        }
        private async Task OnReady()
        {




            _timer = new Timer(async _ =>
                {
                    foreach (var channel in client.Guilds.Select(g => g.DefaultChannel))
                    {
                        await channel.SendMessageAsync("Hello there");
                    }



                },
                state: null,
                dueTime: TimeSpan.FromSeconds(0),
                period: TimeSpan.FromSeconds(1)
            );




        }
        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pongggg!");
            }


        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
