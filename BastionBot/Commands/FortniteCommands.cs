using BastionSuperBot.Models;
using Discord.Commands;
using FortniteApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BastionSuperBot.Commands
{
    [Name("Fortnite")]
    [Remarks("2")]
    public class FortniteCommands : ModuleBase
    {
        private DbContext _Db;
        private FortniteClient FortniteClient;

        public FortniteCommands(FortniteClient Client)
        {
            FortniteClient = Client;
        }
        [Command("fnview")]
        [Summary("View your Fortnite profile")]
        public async Task Ping()
        {
            Console.WriteLine(await FortniteClient.FindPlayerAsync(0, "KarlNguyen"));
        }
    }
}
