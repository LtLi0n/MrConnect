namespace LionLibrary.Network
{
    public interface IServiceConnectionConfig
    {
        string ServerName { get; }
        string Host { get; }
        int Port { get; }
        string CertName { get; }
        string PingRoute { get; }
    }
}
