using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotTest2.Models;
using BotTest2.WebHandler;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BotTest2.Modules
{
    static class Overwatch
    {
      
        public static async Task View(ISocketMessageChannel Channel, User User)
        {
            if (User.OverwatchProfile == null)
            {
                await Channel.SendMessageAsync("You currently don't have an Overwatch profile.\nType !owcreate to make one.");
                return;
            }
            OverwatchViewModel Data = await OverwatchScraper.GetOverwatchData();
            
            await Channel.SendMessageAsync(Data.Level);
        }
        public static async Task Create(ISocketMessageChannel Channel, DbContext Db, User User, List<User> Users,string BattleTag)
        {
            User.OverwatchProfile = new OverwatchProfile
            {
                BattleTag = BattleTag
            };
            Db.writeObject(Users);
            await Channel.SendMessageAsync("Account created");
        }
    }
}
