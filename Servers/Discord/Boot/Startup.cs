using LionLibrary.Commands;
using LionLibrary.Framework;
using LionLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServerDiscord.Services;
using LionLibrary.Network;
using DataServerHelpers;

namespace ServerDiscord.Boot
{
    public class Startup
    {
        public ReadOnlyCollection<string> Args { get; }
        private readonly IServiceProvider _services;

        public Startup(string[] args)
        {
            Args = new ReadOnlyCollection<string>(args);
            _services = ConfigureServices();
            Console.OutputEncoding = Encoding.UTF8;
        }

        private IServiceProvider ConfigureServices()
        {
            ServiceCollection sc = new ServiceCollection();
            sc.RegisterLionLibraryTypes();
            AppConfig config = new AppConfig();
            {
                sc.AddSingleton<IConnectionStringConfig>(config);
                sc.AddSingleton<IDataModuleConfig>(config);
                sc.AddSingleton<IServerConfig>(config);
                sc.AddSingleton<ISslServerConfig>(config);
            }
            sc.AddSingleton<LionServer<SocketUser, CustomCommandContext>>();
            sc.AddTransient<DataContext>();

            return sc.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            ILogService logger = _services.GetService<ILogService>();
            logger.LogLevel = LogSeverity.Debug;
            logger.CursorVisible = false;
            logger.Start();

            await _services.GetService<ICommandService>().InstallCommandsAsync(Assembly.GetExecutingAssembly(), _services);
            var server = _services.GetService<LionServer<SocketUser, CustomCommandContext>>();
            server.Init(x => new SocketUser(x), (cService, sockUser, packet) => new CustomCommandContext(cService, sockUser, packet));
            server.Start(_services);

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
            }

            await Task.Delay(-1);
        }
    }
}
