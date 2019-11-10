using System;

namespace LionLibrary.Utils
{
    //https://github.com/discord-net/Discord.Net/blob/dev/src/Discord.Net.Core/Utils/Preconditions.cs
    //owner: discord.net 
    //under MIT license.
    public static class Preconditions
    {
        public static void NotNull<T>(T obj, string name, string msg = null) where T : class { if (obj == null) throw CreateNotNullException(name, msg); }
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> must not be <see langword="null"/>.</exception>
        public static void NotNull<T>(Optional<T> obj, string name, string msg = null) where T : class { if (obj.IsSpecified && obj.Value == null) throw CreateNotNullException(name, msg); }

        private static ArgumentNullException CreateNotNullException(string name, string msg)
        {
            if (msg == null) return new ArgumentNullException(paramName: name);
            else return new ArgumentNullException(paramName: name, message: msg);
        }
    }
}
