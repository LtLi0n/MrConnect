﻿using Discord.Commands;
using Discord.Rest;
using System.Threading.Tasks;
using LionLibrary.Network;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Discord.Shared;

using UserApi = Discord.Shared.UserApi;
using Discord;

namespace MrConnect.Server.Discord
{
    [Group("discord")]
    public class DiscordModule : ModuleBase<SocketCommandContext>
    {
        public DiscordConnector Discord { get; }

        public DiscordModule(DiscordConnector discordConn)
        {
            Discord = discordConn;
        }

        [Command("add_user")]
        [RequireOwner]
        public async Task AddUserAsync(IUser user)
        {
            var reply = await Discord.GetController<UserApi>().CRUD.AddAsync(
                new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    Discriminator = user.Discriminator
                });

            await ReplyAsync(reply.Content);
        }

        [Command("cmd", RunMode = RunMode.Async)]
        [RequireOwner]
        public async Task CmdAsync([Remainder]string query)
        {
            string[] inputs = query.Split(' ');

            Packet response = await Discord.DownloadPacketAsync(x =>
            {
                x.Header = inputs[0];

                //parse args
                if (query.TrimEnd().Length > x.Header.Length)
                {
                    string arg_str = query.Substring(x.Header.Length);
                    string regex_str = @$" *([^=]+)=(?(?="")(""([^""]+)"")|([^ ]+))";
                    MatchCollection arg_matches = Regex.Matches(arg_str, regex_str);
                    foreach (Match match in arg_matches)
                    {
                        if (match.Success)
                        {
                            x.Args.Add(match.Groups[1].Value, (match.Groups[2].Success ? match.Groups[2] : match.Groups[3]).Value);
                        }
                    }
                }
            });

            string msg = response.Content;

            if (response.ContentType == PacketContentType.JSON)
            {
                msg = $"```json\n{JToken.Parse(response.Content).ToString()}```";
            }

            await ReplyAsync(msg);
        }
    }
}
