using BotTest2.Models;

using BotTest2.WebHandler;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BotTest2.Commands
{
    public class OWCommands : ModuleBase
    {
        private DbContext _Db;
        private bool IsValidbattleTag(string btag)
        {
            string pattern = @"[A-Za-z]([A-Za-z]|[0-9]){2,11}\#[0-9]{1,6}";
            Regex r = new Regex(pattern);
            if (r.IsMatch(btag))
            {
                return true;
            }
            return false;
        }
        public OWCommands(DbContext Db)
        {
            _Db = Db;
        }


        [Command("owview", RunMode = RunMode.Async), Summary("View Overwatch profile.")]
        public async Task View([Remainder, Summary("BattleTag")] string BattleTag = null )
        {
            using (Context.Channel.EnterTypingState())
            {
                OverwatchViewModel Data;
                OverwatchProfile OverwatchProfile;
                User User = _Db.Users.Find(u => u.Id == Context.User.Id);

                //Default value, when command has no parameter
                if (BattleTag == null)
                {
                    
                    if (User.OverwatchProfile == null)
                    {
                        await ReplyAsync("You currently don't have an Overwatch profile");
                        await ReplyAsync("Type !owcreate [BattleTag] to make one");
                        return;
                    }
                    OverwatchProfile = User.OverwatchProfile;

                }
                else
                {
                    //Temporary OW profile
                    OverwatchProfile = new OverwatchProfile
                    {
                        BattleTag = BattleTag,
                        ProfileLink = "https://playoverwatch.com/en-us/career/pc/" + BattleTag.Replace("#", "-")
                    };
                }

                Data = await OverwatchScraper.GetOverwatchData(OverwatchProfile.ProfileLink, BattleTag);


                if (Data == null) //TODO: if Data == null, it might be the scraper is not working, not that the user doesn't exist
                {
                    await ReplyAsync(OverwatchProfile.BattleTag + " does not exist.");
                    return;
                }

                //The general template of an OW response
                var embedBuilder = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        Name = OverwatchProfile.BattleTag,
                        Url = OverwatchProfile.ProfileLink
                    },
                    Color = new Color(255, 176, 51),
                    Url = OverwatchProfile.ProfileLink,
                    Title = "View full stats",
                    Timestamp = DateTime.Now,
                    ThumbnailUrl = Data.PortraitLink,
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = "HanzoMain™"
                    }
                };
                embedBuilder.AddField("Level", Data.Level);
                embedBuilder.AddField("Rank points", Data.RankPoint);
                Embed Embed = embedBuilder.Build();


                await Context.Channel.SendMessageAsync("", false,  Embed);
            }

        }


        [Command("owcreate"), Summary("Create Overwatch profile")]
        public async Task Create([Remainder, Summary("BattleTag")] string BattleTag)
        {
            if (!IsValidbattleTag(BattleTag))
            {
                await ReplyAsync("Invalid BattleTag");
                return;
            }

            User User = _Db.Users.Find(u => u.Id == Context.User.Id);
            User.OverwatchProfile = new OverwatchProfile
            {
                BattleTag = BattleTag,
                ProfileLink = "https://playoverwatch.com/en-us/career/pc/" + BattleTag.Replace("#","-")
            };
            _Db.SaveChanges();
            await ReplyAsync("Account created");
        }

        


    }
}
