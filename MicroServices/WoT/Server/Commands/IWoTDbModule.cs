using LionLibrary.Commands;
using Microsoft.EntityFrameworkCore;
using WoT.Server.Services;

namespace WoT.Server.Commands
{
    ///<summary>Use it to not forget mandatory methods for back-end data filling.</summary>
    ///<typeparam name="T">Data object.</typeparam>
    public interface IWoTDbModule<T> : IDataModule<T> where T : class
    {
        WoTDbContext SQL { get; set; }
        DbContext DbContext { get; }
    }
}
