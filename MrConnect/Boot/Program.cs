using System.Threading.Tasks;
using MrConnect.Boot;

namespace MrConnect
{
    class Program
    {
        static async Task Main(string[] args) => await new Startup(args).StartAsync();
    }
}
