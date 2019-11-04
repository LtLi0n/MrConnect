using LionLibrary.Commands;
using Microsoft.EntityFrameworkCore;

namespace Discord.Server.Network.Commands
{
    ///<summary>Use it to not forget mandatory methods for back-end data filling.</summary>
    ///<typeparam name="T">Data object.</typeparam>
    public interface IDiscordDbModule<T> : IDataModule<T> where T : class
    {
        DiscordDbContext SQL { get; set; }
        DbContext DbContext { get; }
    }
}
