using BotTest2.Models;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using BotTest2.Datas;

namespace BotTest2
{
    class Program
    {
        private CommandService commands;
        private IServiceProvider services;
        private ServiceCollection _serviceCollection;
        public DbContext Db { get; set; }
        public Timer _timer { get; set; }
        public DiscordSocketClient client { get; set; }

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            _serviceCollection = new ServiceCollection();
            Db = new DbContext();

            services = new ServiceCollection()
                .AddSingleton(Db)
                .AddSingleton(client)
                .BuildServiceProvider();
            await InstallCommands();
            string token = "Mzk5ODc3OTI5NDM5MzMwMzA2.DX5_cQ.lDQUWxs6hs-jcY2LiCibsG54azw";
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();


            // Block this task until the program is closed.
            await Task.Delay(-1);

        }

        public async Task InstallCommands()
        {

            client.Log += Log;
            client.MessageReceived += HandleCommand;
            client.UserJoined += UpdateUser;
            client.Ready += OnReady;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        private async Task OnReady()
        {
            //Grab all users
            var UsersInClient = client.Guilds.SelectMany(g => g.Users).Where(u => !u.IsBot);

            //If user is new, add to Db
            foreach (var user in UsersInClient)
            {
                if (!Db.Users.Select(u => u.Id).Contains(user.Id))
                {
                    Db.Users.Add(new User
                    {
                        Id = user.Id
                    }
                    );
                }
            }
            Db.SaveChanges();






        }
        public async Task HandleCommand(SocketMessage messageParam)
        {

            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            var context = new CommandContext(client, message);
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix(Constants.PREFIX, ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }







        }
        public async Task UpdateUser(SocketGuildUser user)
        {

            if (Db.Users.Select(t => t.Id).Contains(user.Id) | user.IsBot)
            {
                return;
            }
            User u = new User
            {
                Id = user.Id
            };
            Db.Users.Add(u);
            Db.SaveChanges();

        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
