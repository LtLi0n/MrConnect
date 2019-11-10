using LionLibrary.Commands;
using System.Threading.Tasks;

namespace CommandsExample.Modules
{
    [Module("example1")] //Module attribute is mandatory
    public class ExampleModule1 : BetterModuleBase //Inheriting from ModuleBase is also mandatory (BetterModuleBase contains integrated Logger methods)
    {
        [Command("ping")]
        public async Task Ping()
        {
            LogLine("pong!");
        }

        [Command("echo")]
        [MandatoryArguments("text")]
        public async Task Echo()
        {
            LogLine(Args["text"]);
        }

        [Command("sum-int")]
        [MandatoryArguments("a", "b")]
        public async Task SumInt()
        {
            int a = GetArgInt32("a");
            int b = GetArgInt32("b");

            LogLine(a + b);
        }
    }
}
