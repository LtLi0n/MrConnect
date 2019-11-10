using LionLibrary.Commands;
using LionLibrary.Framework;
using LionLibrary.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommandsExample
{
    ///<summary>An entry class.
    ///<para>This example will not use Dependency Injection for simplicity.</para></summary>
    public class Startup
    {
        public ReadOnlyCollection<string> Args { get; }

        ///<summary>LogService is optional, but it is very helpful to maintain a wanted log level.
        ///<para>By changing LogLevel you can swap to production or debug modes back and forth.</para></summary>
        public LogService Logger { get; }

        ///<summary>This is what we are looking for in this example, the CommandService.
        ///<para>CommandService syncs all modules automatically that are specified with the correct syntax, so we don't need to do anything extra, just install the commands.</para>
        ///<para>CommandService is a great tool to abstract hard problems and focus on them only.</para>
        ///<para>CommandService is capable of achieving great back-end alongside with Network namespace in LionLibrary.</para></summary>
        public CommandService Commands { get; }

        public Startup(string[] args)
        {
            Args = new ReadOnlyCollection<string>(args);

            Logger = new LogService
            {
                LogLevel = LogSeverity.Debug, //Give me every log
                CursorVisible = false, //No blinking cursors under my watch!
                InputHinter = ">" //That terminal feel.
            };

            //IgnoreCase - if false, the commands and modules will be case sensitive.
            Commands = new CommandService(ignoreCase: false, logger: Logger);
        }

        public async Task StartAsync()
        {
            Logger.Start(); //Start the logger thread. Without starting it, it will remain zombie-like.

            //Install all modules and commands automatically.
            await Commands.InstallCommandsAsync(Assembly.GetExecutingAssembly());


            await Commands["example1"]["ping"].ExecuteAsync(new Dictionary<string, string>()); //logs 'pong!'
            await SumInt(5, 10); //logs '15'
            await Info();

            await Task.Delay(-1);
        }

        ///<summary>It may seem counter intuitive, but using the commands with arguments, even without network focused application, Packets can be used to call them.</summary>
        private async Task SumInt(int a, int b)
        {
            PacketBuilder pb = new PacketBuilder();
            pb.Header = Commands["example1"]["sum-int"].Header;
            pb["a"] = a;
            pb["b"] = b;

            await Commands[new Packet(pb)].ExecuteAsync();
        }

        private async Task Info()
        {
            PacketBuilder pb = new PacketBuilder();
            pb.Header = Commands["example2"].Modules["inner-nested"]["info"].Header;
            //pb["caller"] = "lion";

            await Commands[new Packet(pb)].ExecuteAsync();
        }
    }
}
