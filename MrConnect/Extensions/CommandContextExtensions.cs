using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;

namespace MrConnect.Server
{
    public static class CommandContextExtensions
    {
        ///<summary></summary>
        /// <param name="context"></param>
        /// <param name="userStr"></param>
        /// <returns></returns>
        public static IEnumerable<IGuildUser> GetAddressedUsers(this SocketCommandContext context, string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new Exception("You must select user by mention, id, or by name/nickname.");
            }
            if (context.Message.MentionedUsers.Count == 1)
            {
                yield return (IGuildUser)context.Message.MentionedUsers.First();
                yield break;
            }
            if (address.IsNumber())
            {
                bool success = ulong.TryParse(address, out ulong id);
                if (!success)
                {
                    throw new Exception("ID string was not converted into a variable.");
                }
                yield return context.Guild.GetUser(id);
                yield break;
            }

            foreach(IGuildUser usr in context.Guild.Users)
            {
                if (usr.Username == address)
                {
                    yield return usr;
                    yield break;
                }
                if (usr.Username.ToLower() == address.ToLower())
                {
                    yield return usr;
                }
                else if (usr.Nickname != null)
                {
                    if (usr.Nickname == address)
                    {
                        yield return usr;
                        yield break;
                    }
                    if (usr.Nickname.ToLower() == address.ToLower())
                    {
                        yield return usr;
                    }
                }
            }
        }
    }
}
