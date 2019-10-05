using DataServerHelpers;
using LionLibrary.Commands;
using System;
using System.Collections.Generic;

namespace WoT.Server.Commands
{
    public class SocketModuleBase<T> : BetterModuleBase<T> where T : CustomCommandContext
    {
        ///<summary>Sends user an object formatted as JSON.</summary>
        public void Reply<U>(IEnumerable<U> objects) => Context.User.ReplyObjects(Context.Packet, Context.Header, objects);

        ///<summary>Sends user an object formatted as JSON.</summary>
        public void Reply(object obj) => Context.User.ReplyObject(Context.Packet, Context.Header, obj);

        public void Reply(string message, bool success = true) => Context.User.ReplyMessage(Context.Packet, Context.Header, message, success);
        public void ReplyError(string message) => Reply(message, false);

        ///<summary>Sends user a reply, that the object was saved successfully.</summary>
        ///<param name="message">A detailed description about the procedure.</param>
        ///<param name="id">Id of the saved object.</param>
        public void Reply(string message, int id) => Context.User.ReplyMessage(Context.Packet, Context.Header, message, id);

        ///<summary>Sends user a reply, that the object was saved successfully.</summary>
        ///<param name="message">A detailed description about the procedure.</param>
        ///<param name="id">Id of the saved object.</param>
        public void Reply(string message, uint id) => Context.User.ReplyMessage(Context.Packet, Context.Header, message, id);

        ///<summary>Sends user a reply, that the object was saved successfully.</summary>
        ///<param name="message">A detailed description about the procedure.</param>
        ///<param name="id">Id of the saved object.</param>
        public void Reply(string message, long id) => Context.User.ReplyMessage(Context.Packet, Context.Header, message, id);

        ///<summary>Sends user a reply, that the object was saved successfully.</summary>
        ///<param name="message">A detailed description about the procedure.</param>
        ///<param name="id">Id of the saved object.</param>
        public void Reply(string message, ulong id) => Context.User.ReplyMessage(Context.Packet, Context.Header, message, id);
    }
}
