using System.Threading.Tasks;
using ServerDiscord.Boot;

namespace ServerDiscord
{
    class Program
    {
        static async Task Main(string[] args) => await new Startup(args).StartAsync();
    }
}
