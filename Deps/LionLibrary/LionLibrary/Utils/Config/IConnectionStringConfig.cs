namespace LionLibrary.Utils
{
    public interface IConnectionStringConfig
    {
        string Server { get; }
        string Database { get; }
        string User { get; }
        string Password { get; }
    }
}
