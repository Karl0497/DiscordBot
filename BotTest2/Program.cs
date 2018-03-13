using BotTest2.Models;
using BotTest2.WebHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using BotTest2.Modules;
using System.Reflection;

namespace BotTest2
{
    class Program
    {
        private CommandService commands;

        public DbContext Db { get; set; }
        public List<User> Users { get; set; }
        public Timer _timer { get; set; }
        public DiscordSocketClient client { get; set; }

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient(); 
            commands = new CommandService();
            Db = new DbContext();
            Users = Db.Users;

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
            var UsersInClient = client.Guilds.SelectMany(g => g.Users).Where(u=>!u.IsBot);
            
            //If user is new, add to Db
            foreach (var user in UsersInClient)
            {
                if (!Users.Select(u=>u.Id).Contains(user.Id.ToString()))
                {
                    Users.Add(new User
                    {
                        Id = user.Id.ToString()
                    }
                    );
                }
            }
            Db.writeObject(Users);

            

            //Console.WriteLine(owScraper.GetLevel());
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
            User User = Users.First(u => u.Id == message.Author.Id.ToString());
            
            switch (command)
            {
                case "!ping":
                    await message.Channel.SendMessageAsync("pong!");
                    break;              
                case "!owview":
                    using (message.Channel.EnterTypingState())
                    {
                        await Overwatch.View(message.Channel, User);
                    }
                   
                    break;
                case "!owcreate":
                    if (args.Length !=2){
                        await message.Channel.SendMessageAsync("Invalid command");
                        break;
                    }
                    await Overwatch.Create(message.Channel, Db, User, Users, args[1]);
                    break;
                    
            }

            
        }
        public async Task UpdateUser(SocketGuildUser user)
        {
            
            if (Users.Select(t => t.Id).Contains(user.Id.ToString()) | user.IsBot){
                return;
            }
            User u = new User
            {
                Id = user.Id.ToString()
            };
            Users.Add(u);
            Db.writeObject(Users);
            
        }
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
