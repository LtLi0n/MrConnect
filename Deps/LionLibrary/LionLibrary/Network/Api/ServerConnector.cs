using LionLibrary.Framework;
using LionLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace LionLibrary.Network
{
    public abstract class ServerConnector : IServiceConnectionConfig, IPacketPipe
    {
        private readonly IServiceProvider _services;
        private readonly IServiceProvider _controllers;
        private readonly IServiceConnectionConfig _config;
        public LionClient Client { get; }
        public ILogService Logger { get; }

        public string ServerName => _config.ServerName;
        public string Host => _config.Host;
        public int Port => _config.Port;
        public string CertName => _config.CertName;
        public string PingRoute => _config.PingRoute;

        public event Func<Task> Connected { add { _connectedEvent.Add(value); } remove { _connectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _connectedEvent;

        public ServerConnector(IServiceProvider services, IServiceConnectionConfig config, ILogService logger)
        {
            _services = services;
            _config = config;
            Logger = logger;

            _connectedEvent = new AsyncEvent<Func<Task>>();
            Client = new LionClient(Host, Port, CertName, logger);

            //Routes
            {
                ServiceCollection routes_builder = new ServiceCollection();
                CreateApiControllers(routes_builder);
                _controllers = routes_builder.BuildServiceProvider();
            }
        }

        public T GetController<T>() where T : ApiController => _controllers.GetRequiredService<T>();

        protected abstract void CreateApiControllers(ServiceCollection routes);

        public async Task StartAsync()
        {
            Client.Connected += () => _connectedEvent?.InvokeAsync();
            await Client.ConnectAsync();
            Client.Start();
        }

        public async Task<StatusCode> Ping()
        {
            try
            {
                if (NetworkEssentials.IsConnected(Client.Client.Client))
                {
                    Task<Packet> response_task = Client.DownloadPacketAsync(x =>
                    {
                        x.Header = _config.PingRoute;
                    });

                    await Task.Delay(1500);
                    if (!response_task.IsCompleted)
                    {
                        return StatusCode.Timeout;
                    }
                    Packet response = await response_task;

                    return response.Status;
                }

                return StatusCode.NotConnected;

            }
            catch (Exception ex)
            {
                Logger?.LogLine(this, ex.Message, LogSeverity.Error);
                return StatusCode.NotConnected;
            }
        }

        public void SendPacket(PacketBuilder pb) => Client.SendPacket(pb);
        public void SendPacket(Action<PacketBuilder> pb_action) => Client.SendPacket(pb_action);

        public async Task<Packet> DownloadPacketAsync(Action<PacketBuilder> pb_action) => await Client.DownloadPacketAsync(pb_action);
        public async Task<Packet<T>> DownloadPacketAsync<T>(Action<PacketBuilder> pb_action) => await Client.DownloadPacketAsync<T>(pb_action);

        public async Task<Packet> DownloadPacketAsync(PacketBuilder pb) => await Client.DownloadPacketAsync(pb);
        public async Task<Packet<T>> DownloadPacketAsync<T>(PacketBuilder pb) => await Client.DownloadPacketAsync<T>(pb);
    }
}
