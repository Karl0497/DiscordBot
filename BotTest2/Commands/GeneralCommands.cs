using BotTest2.Models;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotTest2.Commands
{
    public class GeneralCommands : ModuleBase
    {
        private DbContext _Db;

        public GeneralCommands(DbContext Db)
        {
            _Db = Db;
        }
        [Command("ping")]

        public async Task Ping()
        {
            await ReplyAsync("pong");
        }
    }
}
