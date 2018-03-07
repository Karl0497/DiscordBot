using BotTest2.Models;
using Discord;
using Discord.Commands;
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
            client.MessageReceived += HandleCommand;
            client.UserJoined += UpdateUser;
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




            //_timer = new Timer(async _ =>
            //    {
            //        foreach (var channel in client.Guilds.Select(g => g.DefaultChannel))
            //        {
            //            await channel.SendMessageAsync("Hello thereeeee");
            //        }



            //    },
            //    state: null,
            //    dueTime: TimeSpan.FromSeconds(0),
            //    period: TimeSpan.FromSeconds(1)
            //);




        }
        public async Task HandleCommand(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;

            if (message == null) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            string[] args = message.Content.Split();
            string command = args[0];
            switch (command)
            {
                case "!ping":
                    await message.Channel.SendMessageAsync("pong!");
                    break;
                case "!myow":
                    if (Users.First(u => u.Id == message.Author.Id.ToString()) == null)
                    {
                        await message.Channel.SendMessageAsync("You currently don't haveeeeeee an overwatch profile");
                    }
                    break;
            }

            
        }
        public async Task UpdateUser(SocketGuildUser user)
        {

        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
