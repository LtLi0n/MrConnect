using Discord.Commands;
using SharedDiscord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SharedDiscord;
using MrConnect.Services;
using LionLibrary.Network;

using static SharedDiscord.User.Ref;
using Newtonsoft.Json.Linq;
using System.Linq;
using Discord;
using MrConnect.Boot;
using System.IO;
using LionLibrary.Framework;

namespace MrConnect.Discord.Commands
{
    [Group("discord")]
    public class DiscordModule : ModuleBase<SocketCommandContext>
    {
        [Group("user")]
        public class UserModule : ModuleBase<SocketCommandContext>
        {
            [NamedArgumentType]
            public class NamedArgumentHandler
            {
                public ulong? Id { get; set; }
                public ulong? GuildId { get; set; }
            }

            public ConnectorDiscord Discord { get; set; }
            public IAppConfig Config { get; set; }
            public ILogService Logger { get; set; }

            [Command("register")]
            public async Task Register(NamedArgumentHandler args)
            {
                if (args.GuildId.HasValue && Context.User.Id == Config.OwnerId)
                {
                    var users = Context.Guild.Users;
                    Dictionary<ulong, string> failed = new Dictionary<ulong, string>();

                    foreach (var usr in users)
                    {
                        try
                        {
                            Packet response = await RegisterUser(usr.Id);
                            if (response.State != 0) failed.Add(usr.Id, response.Content);
                            Logger.LogLine(this, response.Content, response.State == 0 ? LionLibrary.Framework.LogSeverity.Verbose : LionLibrary.Framework.LogSeverity.Warning);
                        }
                        catch (Exception ex)
                        {
                            failed.Add(usr.Id, ex.Message);
                            Logger.LogLine(this, ex.Message, LionLibrary.Framework.LogSeverity.Warning);
                        }
                    }

                    if (failed.Count > 0)
                    {
                        await ReplyAsync($"Failed to add {failed.Count}/{users.Count} users.");
                    }
                    else
                    {
                        await ReplyAsync("All users were added successfully.");
                    }
                }
            }

            private async Task<Packet> RegisterUser(ulong user_id)
            {
                PacketBuilder pb = new PacketBuilder
                {
                    Header = "user.add"
                };

                IUser discordUser = Context.Client.GetUser(user_id);
                if (discordUser == null)
                {
                    throw new Exception($"Such user doesn't exist.");
                }
                if (discordUser.IsBot)
                {
                    throw new Exception($"This user is a bot.");
                }

                pb[Id] = discordUser.Id;
                pb[Username] = discordUser.Username;
                pb[Discriminator] = discordUser.Discriminator;

                return await Discord.Client.DownloadPacketAsync(pb);
            }

            [RequireOwner]
            [Command("get")]
            public async Task Get(User user = null)
            {
                Packet response = await Discord.Client.DownloadPacketAsync(x =>
                {
                    x.Header = "user.get";

                    if (user?.Id != null)
                    {
                        x[Id] = user.Id;
                    }
                });

                string content = JToken.Parse(response.Content).ToString();

                if (user == null)
                {
                    using MemoryStream ms = new MemoryStream();
                    using StreamWriter sw = new StreamWriter(ms);
                    sw.Write(content);
                    sw.Flush();
                    ms.Position = 0;

                    await Context.Channel.SendFileAsync(ms, "users.json");

                    sw.Close();
                    ms.Close();
                }
                else
                {
                    await ReplyAsync($"```json\n{content}```");
                }
            }
        }
    }
}
