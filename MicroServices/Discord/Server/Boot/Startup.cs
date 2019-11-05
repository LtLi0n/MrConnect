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
using MrConnect.Shared;

namespace Discord.Server
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
            sc.AddSingleton<MrConnectConnector>();

            AppConfig config = new AppConfig();
            {
                sc.AddSingleton(config);
                sc.AddSingleton(config.MrConnectServiceConnectionConfig);
                sc.AddSingleton<IServerConfig>(config);
                sc.AddSingleton<ISslServerConfig>(config);
                sc.AddSingleton<IDataModuleConfig>(config);
                sc.AddSingleton<IConnectionStringConfig>(config);
            }

            sc.AddSingleton<DiscordServerService>();
            sc.AddDbContext<DiscordDbContext>(
                x => DiscordDbContext.UseMySqlOptions(x, config),
                contextLifetime: ServiceLifetime.Transient);

            return sc.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            ILogService logger = _services.GetService<ILogService>();
            logger.Start();
            logger.CursorVisible = false;
            logger.LogLevel = LionLibrary.Framework.LogSeverity.Verbose;

            await _services.GetService<ICommandService>()
                .InstallCommandsAsync(Assembly.GetExecutingAssembly(), _services);

            //Init server
            _services.GetService<DiscordServerService>().Init(
                client => new Network.SocketDiscordUser(client),
                (cService, user, packet) => new DataServerHelpers.CustomCommandContext(
                    _services.GetService<ICommandService>(),
                    user,
                    packet));

            //Start server
            _services.GetService<DiscordServerService>().Start(_services);

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
