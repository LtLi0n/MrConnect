using LionLibrary.Framework;
using System;
using System.Threading.Tasks;

namespace WoT.Server
{
    public interface IWoTService
    {
        ILogService Logger { get; }
        Task UpdateAsync(IServiceProvider services);
    }
}
