using LionLibrary.Framework;
using LionLibrary.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataServerHelpers
{
    public abstract class ServerConnector : IServiceConnectionConfig
    {
        private readonly IServiceProvider _services;
        private readonly IServiceConnectionConfig _config;
        public LionClient Client { get; }
        public ILogService Logger { get; }

        public string ServerName => _config.ServerName;
        public string Host => _config.Host;
        public int Port => _config.Port;
        public string CertName => _config.CertName;

        public ServerConnector(IServiceProvider services, IServiceConnectionConfig config, ILogService logger)
        {
            _services = services;
            _config = config;
            Logger = logger;
            Client = new LionClient(Host, Port, CertName, logger);
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
                if (NetworkEssentials.IsConnected(Client.Client.Client))
                {
                    Task<Packet> response_task = Client.DownloadPacketAsync(x =>
                    {
                        x.Header = "core.ping";
                    });

                    await Task.Delay(1500);
                    if (!response_task.IsCompleted)
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
