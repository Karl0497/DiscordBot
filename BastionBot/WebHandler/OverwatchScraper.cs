using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BastionSuperBot.Datas;
using HtmlAgilityPack;

namespace BastionSuperBot.WebHandler
{
    public class OverwatchViewModel
    {
        public string BattleTag { get; set; }
        public string Level { get; set; }
        public string RankPoint { get; set; }
        public string PortraitLink { get; set; }
        public string Rank { get; set; }
        public string FavouriteHeroImage { get; set; }
        public string TimePlayed { get; set; }
        public string GamesWon { get; set; }

    }
    static class OverwatchScraper
    {
        public static HtmlDocument HtmlDoc { get; set; }
        private static bool IsValidAccount()
        {
            var playerPortrait = HtmlDoc.DocumentNode.SelectSingleNode("//img[@class='player-portrait']");
            if (playerPortrait != null)
            {
                return true;
            }
            return false;
        }
        public static async Task<OverwatchViewModel> GetOverwatchData(string Uri, string btag)
        {
            HtmlWeb web = new HtmlWeb();
            await Task.Delay(5000);
            HtmlDoc = await web.LoadFromWebAsync(Uri);
            if (!IsValidAccount())
            {
                return null;
            }
            return new OverwatchViewModel
            {
                BattleTag = btag,
                Level = GetLevel(),
                RankPoint = GetRankPoint(),
                PortraitLink = GetPortraitLink(),
                Rank = GetRank(),
                FavouriteHeroImage = GetHeroLink(),
                TimePlayed = GetTimePlayed(),
                GamesWon = GetGamesWon()
            };
        }

        public static string GetPortraitLink()
        {
            var playerPortrait = HtmlDoc.DocumentNode.SelectSingleNode("//img[@class='player-portrait']");
            return playerPortrait.GetAttributeValue("src", null); //TODO: replace default value null
        }
        public static string GetRankPoint()
        {
            var RankPointDiv = HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='competitive-rank']");
            if (RankPointDiv != null)
            {
                return RankPointDiv.InnerText;
            }
            return null;
        }
        public static string GetLevel()
        {
            int Level = -1;
            var LevelDiv = HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='player-level']//div[@class='u-vertical-center']").InnerText;
            Level = Convert.ToInt32(LevelDiv);
            var LevelRankDiv = HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='player-level']//div[@class='player-rank']");
            if (LevelRankDiv != null)
            {
                //Remove Extension
                var LevelRank = LevelRankDiv.GetAttributeValue("style", null).Replace("_Rank.png)", "");
                LevelRank = LevelRank.Substring(LevelRank.Length - 18);
                foreach (var level in Constants.LEVEL_IDS)
                {
                    if (level.Value.Contains(Convert.ToInt64(LevelRank, 16)))
                    {
                        Level += level.Key;
                    }
                }
            }

            return Level.ToString();
        }
        public static string GetRank()
        {
            string[] Ranks = { "Bronze", "Silver", "Gold", "Platinum", "Diamond", "Master", "Grand Master" };
            var RankImg = HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='competitive-rank']//img");
            if (RankImg != null)
            {
                string ImgLink = RankImg.GetAttributeValue("src", null); //TODO: replace default value null

                int RankIndex = Convert.ToInt32(ImgLink.Substring(ImgLink.Length - 5, 1));
                return Ranks[RankIndex - 1];

            }
            return null;
        }
        public static string GetHeroLink()
        {
            var HeroDiv = HtmlDoc.DocumentNode.SelectSingleNode("//div[@data-js='heroMastheadImage']");
            string HeroName = HeroDiv.GetAttributeValue("data-hero-quickplay", null); //TODO: replace default value null
            return "https://d1u1mce87gyfbn.cloudfront.net/hero/" + HeroName + "/career-portrait.png";
        }
        public static string GetTimePlayed()
        {
            return GetTableData("Game", 0);
        }
        public static string GetTableData(string TableName, int RowNumber)
        {
            var AllTables = HtmlDoc.DocumentNode.SelectNodes("//div[@class='card-stat-block']//table").Take(7); //Each hero (and All Heroes) has 7 cards
            
            var Table = AllTables.First(t => t.SelectSingleNode(".//h5").InnerText == TableName);
            var rows = Table.SelectNodes(".//tbody//tr");
            
            
            if (rows.Count <= RowNumber)
            {
                return null;
            }

            var data = rows[RowNumber].SelectNodes(".//td")[1].InnerText;
            return data;

        }
        public static string GetGamesWon()
        {
            return GetTableData("Game", 1);
        }

    }
}
