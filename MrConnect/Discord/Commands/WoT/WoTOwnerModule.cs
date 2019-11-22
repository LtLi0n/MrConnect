using Discord.Commands;
using System.Threading.Tasks;
using WoT.Shared;
using LionLibrary.Network;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using MrConnect.Server.Boot;
using MrConnect.Server.Discord;

namespace MrConnect.Discord.Commands
{
    [Group("owner", ParentModule = typeof(WoTModule)), RequireOwner]
    public class WoTOwnerModule : ModuleBase<SocketCommandContext>
    {
        public AppConfig Config { get; }
        public WoTConnector WoT { get; }

        public UserApi UserApi { get; }
        public CharacterApi CharacterApi { get; }
        public CharacterWorkApi CharacterWorkApi { get; }

        public WoTOwnerModule(AppConfig appConfig, WoTConnector wotConn)
        {
            Config = appConfig;
            WoT = wotConn;

            UserApi = wotConn.GetController<UserApi>();
            CharacterApi = wotConn.GetController<CharacterApi>();
            CharacterWorkApi = wotConn.GetController<CharacterWorkApi>();
        }

        [Command(RunMode = RunMode.Async)]
        public async Task CmdAsync([Remainder]string query)
        {
            string[] inputs = query.Split(' ');

            Packet response = await WoT.DownloadPacketAsync(x =>
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
