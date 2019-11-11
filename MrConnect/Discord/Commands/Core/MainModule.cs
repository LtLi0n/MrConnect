using MrConnect.Server.Boot;
using Discord;
using Discord.Rest;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataServerHelpers;
using WoT.Shared;
using LionLibrary.Network;
using System.Linq;
using Discord.WebSocket;
using System;
using System.Text;
using Discord.Shared;

using Emoji = NeoSmart.Unicode.Emoji;
using System.Diagnostics.Eventing.Reader;

namespace MrConnect.Server.Discord
{
    public class MainModule : ModuleBase<SocketCommandContext>
    {
        public AppConfig Config { get; set; }
        public WoTConnector WoT { get; set; }
        public DiscordConnector Discord { get; set; }

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong!");
        }

        [Command("say")]
        public async Task SayAsync([Remainder]string text)
        {
            await Context.Message.DeleteAsync();

            await ReplyAsync(text);
        }

        [Command("user")]
        [Summary(
            "%Regular%%info%User info command.%details%\n" +
            "Info about you or targetted user.\n" +
            "%usage%`%prefix%%cmd% ([user_name/@mention/id])`\n" +
            "%examples%\t\t`%prefix%%cmd%` -- Shows info about you.\n" +
            "\t\t`%prefix%%cmd% Bob123` -- Shows info about targetted user.§")]
        [RequireContext(ContextType.Guild)]
        public async Task DisplayGuildUserInfoAsync(IGuildUser user = null)
        {
            user ??= (IGuildUser)Context.User;
            string username = user.ToString();

            EmbedBuilder eb = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = username,
                    IconUrl = user.Status switch
                    {
                        UserStatus.Online => $"https://cdn.discordapp.com/emojis/630527350173466655.png",
                        UserStatus.DoNotDisturb => "https://cdn.discordapp.com/emojis/630527350286843904.png",
                        UserStatus.Offline => "https://cdn.discordapp.com/emojis/630527349917745153.png",
                        UserStatus.Invisible => "https://cdn.discordapp.com/emojis/630527349917745153.png",
                        UserStatus.Idle => "https://cdn.discordapp.com/emojis/630536692654145556.png",
                        UserStatus.AFK => "https://cdn.discordapp.com/emojis/630536692654145556.png",
                        _ => "https://cdn.discordapp.com/emojis/643114599893434379.png"
                    }
                },
                ThumbnailUrl = user.GetAvatarUrl(),
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder
                    {
                        Name = $"{Emoji.Information} User Information {Emoji.Information}",
                        Value =
                        $"```swift\n" +
                        $"{Emoji.Calendar}  Created User At: {user.CreatedAt.ToString("d")}\n" +
                        $"{Emoji.Calendar} Joined Server At: {user.JoinedAt.Value.ToString("d")}\n" +
                        $"{Emoji.Key} Guild Perms: {user.GuildPermissions}```",
                        IsInline = true
                    }
                }
            };

            if(!string.IsNullOrEmpty(user.Nickname))
            {
                eb.Fields.Insert(0, new EmbedFieldBuilder
                {
                    Name = "Nickname",
                    Value = user.Nickname == null ? "Not Set" : $"{user.Nickname}",
                    IsInline = true
                });
            }

            if(user.Activity != null)
            {
                if (user.Activity.Type == ActivityType.Listening)
                {
                    //Override description by spotify details (album and producer urls are not provided sadly)
                    if (user.Activity is SpotifyGame spotifyActivity)
                    {
                        TimeSpan elapsed = spotifyActivity.Elapsed.Value;
                        TimeSpan duration = spotifyActivity.Duration.Value;

                        int percentange_elapsed = (int)Math.Ceiling(elapsed.Ticks * 100.0M / duration.Ticks);

                        //create playing song progress line
                        char[] progress = new string('-', 11).ToCharArray();
                        {
                            char[] circle_emoji = Emoji.RadioButton.ToString().ToCharArray();
                            int emoji_index = percentange_elapsed / 10;

                            progress[emoji_index] = circle_emoji[0];
                            progress[emoji_index + 1] = circle_emoji[1];
                        }


                        eb.Description =
                            $"{Emoji.MusicalNote} Listening to **[{spotifyActivity.TrackTitle}]({spotifyActivity.TrackUrl})**\n" +
                            $"```swift\n{new string(progress)} {elapsed.ToString(@"mm\:ss")}/{duration.ToString(@"mm\:ss")}```\n" +
                            $"By **{string.Join(", ", spotifyActivity.Artists)}**\n" +
                            $"On **{spotifyActivity.AlbumTitle}**";

                        eb.ImageUrl = spotifyActivity.AlbumArtUrl;
                    }
                }
            }

            //Add roles
            if (user.RoleIds.Count > 0)
            {
                eb.Color = Context.Guild.GetRole(user.RoleIds.First()).Color;

                List<IRole> userRoles = new List<IRole>();

                foreach (ulong roleID in user.RoleIds)
                {
                    if (Context.Guild.GetRole(roleID).IsEveryone)
                        continue;

                    userRoles.Add(Context.Guild.GetRole(roleID));
                }

                IEnumerable<string> sortedRoles = userRoles
                    .OrderByDescending(x => x.Position)
                    .Select(x => $"<@&{x.Id}>");

                EmbedFieldBuilder roleField = new EmbedFieldBuilder
                {
                    Name = string.Format("Role{0}", sortedRoles.Count() > 1 ? $"s [{sortedRoles.Count()}]" : string.Empty)
                };

                roleField.Value = string.Join(", ", sortedRoles);

                eb.AddField(roleField);
            }

            eb.Footer = new EmbedFooterBuilder { Text = $"Id: {user.Id}" };

            await Context.Channel.SendMessageAsync(embed: eb.Build());
        }

        private void ReflectServerResponse(EmbedFieldBuilder efb, StatusCode state) =>
            efb.Value = state switch
            {
                StatusCode.NotConnected => "❌ Offline",
                StatusCode.Timeout => "⚠️ Not responding",
                StatusCode.Success => "🌐 Online",
                _ => "Failure"
            };

        [Command("status", RunMode = RunMode.Async)]
        public async Task Status()
        {
            RestUserMessage msg = null;
            EmbedBuilder eb = new EmbedBuilder();

            //GuildEmoji processing_emoji = await Discord.GuildEmoji.DownloadProcessingGif();
            string processing_emoji = "<a:processing:603313837730693137>";

            eb.Fields = new List<EmbedFieldBuilder>()
            {
                new EmbedFieldBuilder
                {
                    Name = $"⚔️ WoT Service ⚔️",
                    Value = $"{processing_emoji} Pinging..."
                },
                new EmbedFieldBuilder
                {
                    Name = $"🤔 Discord Service 🤔",
                    Value = $"{processing_emoji} Pinging..."
                }
            };

            //todo, should move them to a generic helper method class, this is yikes

            //WoT Service ping
            {
                msg = (RestUserMessage)await ReplyAsync(embed: eb.Build());

                await PingAndModifyField(WoT, eb.Fields[0]);
                eb.Fields[0].Value += "\n\u200b";
                await msg.ModifyAsync(x => x.Embed = eb.Build());
            }

            //Discord Service ping
            {
                await PingAndModifyField(Discord, eb.Fields[1]);
                eb.Fields[1].Value += "\n\u200b";
                await msg.ModifyAsync(x => x.Embed = eb.Build());
            }

            //WoT Service ping
            /*{
                await PingAndModifyField(WoT, eb.Fields[1]);
                await msg.ModifyAsync(x => x.Embed = eb.Build());
            }*/
        }

        private async Task PingAndModifyField(ServerConnector connector, EmbedFieldBuilder field)
        {
            StatusCode state = await connector.Ping();
            ReflectServerResponse(field, state);
        }

        [Command("info")]
        public async Task Info()
        {
            EmbedBuilder eb = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = Context.Client.CurrentUser.Username,
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                    Url = "https://github.com/LtLi0n/MrConnect"
                },

                Description =
                "MrConnect is currently in very early stages of development.\nn" +
                "The bot will try to achieve a multi purpose role, altough the development focus will shift towards making fun community games.\n\n" +
                "The bot itself will act like a gateway between a user and the servers, meaning that the servers should operate 24/7 independently of the bot.\n\n" +
                "The long term plan is to have many servers, each for different game, so if there are complications with one of them, rest can still operate.",

                Color = Color.Green
            };

            await ReplyAsync(embed: eb.Build());
        }

        [Group("group"), RequireOwner, DontAutoLoad]
        public class GroupModule : ModuleBase<SocketCommandContext>
        {
            public DiscordService Discord { get; set; }

            [Command("set"), Alias("toggle")]
            public async Task SetGroupAsync([Remainder] string path)
            {
                string[] steps = path.Split(' ');
                IEnumerable<ModuleInfo> modules = Discord.Commands.Modules
                    .Where(x => x.Parent == null && !string.IsNullOrEmpty(x.Group));

                for (int i = 0; i < steps.Length; i++)
                {
                    string step = steps[i];
                    ModuleInfo lastModule = null;

                    bool stepWasValid = false;

                    foreach (var module in modules)
                    {
                        if (IsValidStep(step, modules))
                        {
                            lastModule = module;
                            stepWasValid = true;
                            //if there are still more steps to validate.
                            if (i + 1 < steps.Length)
                            {
                                if (lastModule.Submodules.Count > 0)
                                {
                                    modules = lastModule.Submodules;
                                    break;
                                }
                                else
                                {
                                    await ReplyAsync("Path you were trying to opt into does not exist.");
                                    return;
                                }
                            }
                        }
                    }

                    if (!stepWasValid)
                    {
                        await ReplyAsync("Path you were trying to opt into does not exist.");
                        return;
                    }
                }

                await ReplyAsync($"You are now within `{path}` group.\n" +
                    $"You are able to use that group's commands without specifying this path.\n" +
                    $"If you use a command the group does not contain, the bot will attempt to find a command using default settings.\n" +
                    $"You can now inspect your current group with `{Discord.Prefix}group inspect`");
                Discord.AddGroupToggledUser(Context.User, path);
            }

            [Command("inspect")]
            public async Task InspectModuleAsync()
            {
                if (!Discord.GroupToggledUsers.ContainsKey(Context.User.Id))
                {
                    await ReplyAsync(
                        $"You need to use group pathing for this command.\n" +
                        $"Set your group with `{Discord.Prefix}group set`");
                    return;
                }

                string toReturn = string.Empty;
                toReturn += $"My Group: {Discord.GroupToggledUsers[Context.User.Id]}";



                await ReplyAsync(toReturn);
                //todo
            }

            private bool IsValidStep(string step, IEnumerable<ModuleInfo> modules)
            {
                foreach (var module in modules)
                {
                    if (string.IsNullOrEmpty(module.Group)) continue;
                    if (module.Group.ToLower() == step)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
