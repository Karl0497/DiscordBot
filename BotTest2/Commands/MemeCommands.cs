using BotTest2.Models;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotTest2.Commands
{
    public class MemeCommands : ModuleBase
    {
        private DbContext _Db;

        public MemeCommands(DbContext Db)
        {
            _Db = Db;
        }

        

        [Command("ggez")]
        public async Task Ggez()
        {
            await ReplyAsync("https://www.youtube.com/watch?v=u1ejB4YjgKI");
        }
        [Command("DoesAndySuck")]
        public async Task test()
        {
            await ReplyAsync("he sucks big dic");
        }

    }
}
