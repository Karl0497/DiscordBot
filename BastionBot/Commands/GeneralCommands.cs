using BastionSuperBot.Models;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BastionSuperBot.Commands
{
    [Name("Other")]
    [Remarks("98")]
    public class GeneralCommands : ModuleBase
    {
        private DbContext _Db;

        public GeneralCommands(DbContext Db)
        {
            _Db = Db;
        }
        [Command("ping")]
        [Summary("*Pong!*")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }
    }
}
