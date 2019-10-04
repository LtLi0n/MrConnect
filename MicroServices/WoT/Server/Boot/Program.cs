using System.Threading.Tasks;
using WoT.Server.Boot;

namespace WoT.Server
{
    class Program
    {
        static async Task Main(string[] args) => await new Startup(args).StartAsync();
    }
}
