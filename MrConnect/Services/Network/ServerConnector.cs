using LionLibrary.Framework;
using LionLibrary.Network;
using MrConnect.Boot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrConnect.Services
{
    public abstract class ServerConnector : IServerConnector
    {
        private readonly IServiceProvider _services;
        public LionClient Client { get; }
        public ILogService Logger { get; }
        public IAppConfig Config { get; }

        public ServerConnector(IServiceProvider services, IAppConfig config, ILogService logger, string server_name)
        {
            _services = services;
            Config = config;
            Logger = logger;

            Client = new LionClient(
                config[$"servers:{server_name}:host"],
                config.GetValue<int>($"servers:{server_name}:port"),
                config[$"servers:{server_name}:cert_sn"],
                logger);
        }

        public async Task StartAsync()
        {
            await Client.ConnectAsync();
            Client.Start();
        }

        public async Task<int> Ping()
        {
            try
            {
                if(NetworkEssentials.IsConnected(Client.Client.Client))
                {
                    Task<Packet> response_task = Client.DownloadPacketAsync(x =>
                    {
                        x.Header = "core.ping";
                    });

                    await Task.Delay(1500);
                    if(!response_task.IsCompleted)
                    {
                        return -3;
                    }
                    Packet response = await response_task;

                    return response.State;
                }

                return -2;

            }
            catch (Exception ex)
            {
                Logger?.LogLine(this, ex.Message, LogSeverity.Error);
                return -2;
            }
        }
    }
}
