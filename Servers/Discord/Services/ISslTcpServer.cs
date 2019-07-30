using System;

namespace ServerDiscord.Services
{
    public interface ISslTcpServer
    {
        void Start(IServiceProvider services);
        void Stop();
    }
}