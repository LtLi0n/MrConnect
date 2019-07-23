using System.Threading.Tasks;
using LionLibrary.Framework;
using LionLibrary.Network;
using MrConnect.Boot;

namespace MrConnect.Services
{
    public interface IServerCommunicator
    {
        LionClient Client { get; }
        IAppConfig Config { get; }
        ILogService Logger { get; }

        Task<int> Ping();
        Task StartAsync();
    }
}