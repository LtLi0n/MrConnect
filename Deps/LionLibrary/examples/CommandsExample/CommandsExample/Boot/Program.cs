using System.Threading.Tasks;

namespace CommandsExample
{
    class Program
    {
        static async Task Main(string[] args) => await new Startup(args).StartAsync();
    }
}
