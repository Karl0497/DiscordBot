using BastionSuperBot.Models;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using BastionSuperBot.Datas;
using System.IO;
using Newtonsoft.Json.Linq;
using FortniteApi;

namespace BastionSuperBot
{
    class Program
    {

        private CommandService Commands;
        private IServiceProvider Services;
        private ServiceCollection ServiceCollection;
        public DbContext Db;
        public Timer _timer;
        public string Token;
        public string ClientID;
        public DiscordSocketClient Client { get; set; }

        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {

            GetData();
            Client = new DiscordSocketClient();
            Commands = new CommandService();
            ServiceCollection = new ServiceCollection();
            Services = new ServiceCollection()
                .AddSingleton(Db)
                .AddSingleton(Client)
                .BuildServiceProvider();
            await InstallCommands();
            await Client.LoginAsync(TokenType.Bot, Token);
            await Client.StartAsync();


            // Block this task until the program is closed.
            await Task.Delay(-1);

        }

        public async Task InstallCommands()
        {

            Client.Log += Log;
            Client.MessageReceived += HandleCommand;
            Client.UserJoined += UpdateUser;
            Client.Ready += OnReady;
            Commands.Log += LogCommand;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        private async Task OnReady()
        {
            //Grab all users
            var UsersInClient = Client.Guilds.SelectMany(g => g.Users).Where(u => !u.IsBot);

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
            var context = new CommandContext(Client, message);
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;


            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasCharPrefix(Constants.PREFIX, ref argPos) || message.HasMentionPrefix(Client.CurrentUser, ref argPos))) return;
            var result = await Commands.ExecuteAsync(context, argPos, Services);
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
        private Task LogCommand(LogMessage msg)
        {
            if (msg.Exception is CommandException command)
            {
                Console.WriteLine(command.InnerException);
            }
            return Task.CompletedTask;
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        public void GetData()
        {
            using (StreamReader file = new StreamReader("../../../appsettings.json"))
            {

                var jobj = JObject.Parse(file.ReadToEnd());
                Token = jobj["Token"].ToString();
                ClientID = jobj["ClientID"].ToString();


            }
            Db = new DbContext();
            string InviteLink = "https://discordapp.com/api/oauth2/authorize?client_id=" + ClientID + "&scope=bot";
            Console.WriteLine("Use this link to invite your bot: " + InviteLink);
        }
    }
}
