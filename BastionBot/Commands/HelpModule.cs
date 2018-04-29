using BastionSuperBot.Models;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BastionBot.Commands
{
    [Name("Help")]
    [Remarks("0")]
    public class HelpModule : ModuleBase
    {

        private DbContext _Db;
        private CommandService _Commands;

        public HelpModule(DbContext Db, CommandService Commands)
        {
            _Db = Db;
            _Commands = Commands;

        }
        [Command("help")]
        [Summary("Show this")]
        public async Task Help(string path = "")
        {
            path = path.ToLower();
            var embedBuilder = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder()
                {
                    Name = "Help"
                },
                Color = new Color(255, 176, 51),
                Timestamp = DateTime.Now,
                Footer = new EmbedFooterBuilder()
                {
                    Text = "HanzoMain™"
                }
            };
            if (path == "")
            {
                embedBuilder = allModules(embedBuilder);
            }
            else
            {
                if (_Commands.Modules.Select(m => m.Name.ToLower()).Contains(path))
                {
                    embedBuilder = moduleHelp(embedBuilder, path);
                }
                else if (_Commands.Commands.Select(c => c.Name.ToLower()).Contains(path))
                {
                    embedBuilder = commandHelp(embedBuilder, path);
                }
            }

            Embed Embed = embedBuilder.Build();
            await ReplyAsync("", false, Embed);
        }

        public EmbedBuilder commandHelp(EmbedBuilder sample, string path)
        {
            var command = _Commands.Commands.First(c => c.Name.ToLower() == path);
            var allParameters = command.Parameters.Select(p => $"``<{p.Name}>``");

            sample.Title = command.Name + " " +String.Join(" | ",allParameters);


            sample.AddField("Description", command.Summary);

            if (command.Remarks != null)
            {
                sample.AddField("Usage and Example", command.Remarks);
            }
            return sample;

        }
        public EmbedBuilder moduleHelp(EmbedBuilder sample, string path)
        {
            var module = _Commands.Modules.First(m => m.Name.ToLower() == path);
            
            sample.Title = module.Name;
            string info = "";
            
            foreach (var command in module.Commands)
            {
                info += $"**{command.Name}**: "+ command.Summary+"\n";
            }
            sample.AddField("List of commands", info);
            return sample;

        }
        public EmbedBuilder allModules(EmbedBuilder sample)
        {
            sample.Description = "Type !help <category> or <command> for more details. For example ``!help Overwatch``, ``!help owview``";
            string info;
            List<string> allCommands;
            var modules = _Commands.Modules
                .Where(m => m.Remarks != "0")
                .OrderBy(m => Convert.ToInt32(m.Remarks));
            foreach (var module in modules)
            {
                allCommands = module.Commands
                    .Select(c => c.Name)
                    .OrderBy(c => c)
                    .Select(c => $"``{c}``")
                    .ToList();
                info = String.Join(", ", allCommands);
                sample.AddField(module.Name + " \u2014 " + module.Commands.Count().ToString(), info);
            }

            return sample;
        }


    }

}
