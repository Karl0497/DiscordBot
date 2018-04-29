using BastionSuperBot.Models;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BastionSuperBot.Commands
{
    [Name("Memes")]
    [Remarks("99")]
    public class MemeCommands : ModuleBase
    {
        private DbContext _Db;
        
        public MemeCommands(DbContext Db)
        {
            _Db = Db;
        }

        

        [Command("ggez")]
        [Summary("Git Gud")]
        public async Task Ggez()
        {
            await ReplyAsync("https://www.youtube.com/watch?v=u1ejB4YjgKI");
        }
       

    }
}
