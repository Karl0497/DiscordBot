using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTest2.Datas;
using HtmlAgilityPack;

namespace BotTest2.WebHandler
{
    public class OverwatchViewModel
    {
        public string Level { get; set; }

    }
    static class OverwatchScraper
    {

        public static async Task<OverwatchViewModel> GetOverwatchData()
        {
            HtmlWeb web = new HtmlWeb();
            await Task.Delay(5000);
            HtmlDocument doc = await web.LoadFromWebAsync("https://playoverwatch.com/en-us/career/pc/Karl-1194");
            return new OverwatchViewModel
            {
                Level=GetLevel(doc)
            };
        }
        
       
       
        public static string GetLevel(HtmlDocument HtmlDoc)
        {
            int Level = -1;
            var LevelDiv = HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='player-level']//div[@class='u-vertical-center']").InnerText;
            var RankDiv = HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='player-level']//div[@class='player-rank']").GetAttributeValue("style", null);

            //Remove Extension
            RankDiv = RankDiv.Replace("_Rank.png)", "");
            RankDiv = RankDiv.Substring(RankDiv.Length - 18);
            foreach (var level in Constants.LEVEL_IDS)
            {
                if (level.Value.Contains(Convert.ToInt64(RankDiv,16)))
                {
                    Level = level.Key + Convert.ToInt32(LevelDiv);
                }
            }
            return Level.ToString();
        }
    }
}
