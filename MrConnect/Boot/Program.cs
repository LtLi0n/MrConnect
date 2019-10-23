using System.Threading.Tasks;
using MrConnect.Server.Boot;

namespace MrConnect.Server
{
    class Program
    {
        static async Task Main(string[] args) => await new Startup(args).StartAsync();
    }
}
