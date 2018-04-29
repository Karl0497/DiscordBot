using BastionSuperBot.Models;

using BastionSuperBot.WebHandler;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BastionSuperBot.Commands
{
    [Name("Overwatch")]
    [Remarks("1")]
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


        [Command("owview", RunMode = RunMode.Async)]
        [Summary("View Overwatch profile of ``<BattleTag>``, if it is *null*, view your own")]
        [Remarks("``!owview``, ``!owview Karl#1194``")]
        public async Task View(string BattleTag = null)
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
                embedBuilder.AddField("Level", Data.Level,true);
                embedBuilder.AddField("Rank", Data.RankPoint + " - " + Data.Rank,true);
                embedBuilder.AddField("Time Played", Data.TimePlayed, true);
                embedBuilder.AddField("Games Won", Data.GamesWon,true);
                embedBuilder.ImageUrl = Data.FavouriteHeroImage;
                Embed Embed = embedBuilder.Build();

                await ReplyAsync("", false, Embed);
            }

        }


        [Command("owcreate")]
        [Summary("Bind ``<BattleTag>`` to your account to be viewed later by using ``!owview``")]
        [Remarks("``!owcreate Karl#1194``")]
        public async Task Create([Remainder] string BattleTag)
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
                ProfileLink = "https://playoverwatch.com/en-us/career/pc/" + BattleTag.Replace("#", "-")
            };
            _Db.SaveChanges();
            await ReplyAsync("Account created");
        }
    }
}
