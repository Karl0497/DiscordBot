using BotTest2.Models;

using BotTest2.WebHandler;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BotTest2.Commands
{
    public class OWCommands : ModuleBase
    {
        private DbContext _Db;
        public OWCommands(DbContext Db)
        {
            _Db = Db;
        }
        [Command("owview", RunMode = RunMode.Async), Summary("View Overwatch profile.")]
        public async Task View()
        {
          
                User User = _Db.Users.Find(u => u.Id == Context.User.Id);
                if (User.OverwatchProfile == null)
                {
                    await ReplyAsync("You currently don't have an Overwatch profile");
                    await ReplyAsync("Type !owcreate [BattleTag] to make one");
                    return;
                }
                OverwatchViewModel Data = await OverwatchScraper.GetOverwatchData();
                await ReplyAsync(Data.Level);
            
               



        }
        [Command("owcreate"), Summary("Create Overwatch profile")]
        public async Task Create([Remainder, Summary("BattleTag")] string BattleTag)
        {
            User User = _Db.Users.Find(u => u.Id == Context.User.Id);
            User.OverwatchProfile = new OverwatchProfile
            {
                BattleTag = BattleTag
            };
            _Db.SaveChanges();
            await ReplyAsync("Account created");
        }

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }

        
    }
}
