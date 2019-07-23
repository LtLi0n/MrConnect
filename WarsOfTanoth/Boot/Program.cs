using System.Threading.Tasks;
using WarsOfTanoth.Boot;

namespace WarsOfTanoth
{
    class Program
    {
        static async Task Main(string[] args) => await new Startup(args).StartAsync();
    }
}
