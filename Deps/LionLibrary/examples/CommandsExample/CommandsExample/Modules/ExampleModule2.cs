using LionLibrary.Commands;
using System.Threading.Tasks;

namespace CommandsExample.Modules
{
    [Module("example2")] //Module attribute is mandatory
    public class ExampleModule2 : BetterModuleBase //Inheriting from ModuleBase is also mandatory (BetterModuleBase contains integrated Logger methods)
    {
        [Module("inner-nested")]
        public class InnerNestedModuleExample : BetterModuleBase
        {
            [Command("info")]
            [OptionalArguments("caller")]
            public async Task Ping()
            {
                string caller = "caller-default";
                TryFill<string>("caller", x => caller = x); //If caller arg exists, fill it

                LogLine($"[{caller}]: Requested info.");
            }
        }
    }
}
