using System;

namespace WarsOfTanoth.Services
{
    public interface ISslTcpServer
    {
        void Start(IServiceProvider services);
        void Stop();
    }
}