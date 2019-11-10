namespace LionLibrary.Network
{
    public abstract class ApiController
    {
        public ServerConnector Server { get; }
        public LionClient Client => Server.Client;

        public ApiController(ServerConnector server) => Server = server;
    }
}
