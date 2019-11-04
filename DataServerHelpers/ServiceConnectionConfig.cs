using LionLibrary.Network;
using LionLibrary.Utils;

namespace DataServerHelpers
{
    public abstract class ServiceConnectionConfig : IServiceConnectionConfig
    {
        private readonly JsonAppConfigBase _config;

        public string ServerNamePath { get; }
        public string ServerName => _config[ServerNamePath];
        
        public string HostPath { get; }
        public string Host => _config[HostPath];
        
        public string PortPath { get; }
        public int Port => _config.Value<int>(PortPath);

        public string CertNamePath { get; }
        public string CertName => _config[CertNamePath];

        public string PingRoutePath { get; }
        public string PingRoute => _config[PingRoutePath];

        public ServiceConnectionConfig(
            JsonAppConfigBase config, 
            string serverNamePath,
            string hostPath,
            string portPath,
            string certNamePath,
            string pingRoutePath)
        {
            _config = config;

            ServerNamePath = serverNamePath;
            HostPath = hostPath;
            PortPath = portPath;
            CertNamePath = certNamePath;
            PingRoutePath = pingRoutePath;
        }
    }
}
