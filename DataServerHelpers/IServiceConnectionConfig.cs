namespace DataServerHelpers
{
    public interface IServiceConnectionConfig
    {
        string ServerName { get; }
        string Host { get; }
        int Port { get; }
        string CertName { get; }
    }
}
