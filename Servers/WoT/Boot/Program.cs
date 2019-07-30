using System.Threading.Tasks;
using ServerWoT.Boot;

namespace ServerWoT
{
    class Program
    {
        static async Task Main(string[] args) => await new Startup(args).StartAsync();
    }
}
