using System;

namespace ServerWoT.Services
{
    public interface ISslTcpServer
    {
        void Start(IServiceProvider services);
        void Stop();
    }
}