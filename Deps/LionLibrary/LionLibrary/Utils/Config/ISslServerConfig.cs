namespace LionLibrary.Utils
{
    public interface ISslServerConfig : IServerConfig
    {
        string CertFile { get; }
        string CertPassword { get; }
    }
}
