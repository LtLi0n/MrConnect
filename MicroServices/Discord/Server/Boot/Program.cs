using System;
using System.Threading.Tasks;

namespace Discord.Server
{
    class Program
    {
        static async Task Main(string[] args) => await new Startup(args).StartAsync();
    }
}
