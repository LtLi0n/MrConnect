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

namespace WoT.Server.Boot
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
            sc.AddSingleton<UserNotificationService>();

            AppConfig config = new AppConfig();
            {
                sc.AddSingleton(config);
                sc.AddSingleton(config.MrConnectServiceConnectionConfig);
                sc.AddSingleton<IServerConfig>(config);
                sc.AddSingleton<ISslServerConfig>(config);
                sc.AddSingleton<IDataModuleConfig>(config);
                sc.AddSingleton<IConnectionStringConfig>(config);
            }

            sc.AddSingleton<WoTServerService>();
            sc.AddDbContext<WoTDbContext>(
                x => WoTDbContext.UseMySqlOptions(x, config),
                contextLifetime: ServiceLifetime.Transient);

            sc.AddSingleton<CharacterWorkService>();
            sc.AddSingleton<UpdateService>();

            return sc.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            ILogService logger = _services.GetService<ILogService>();
            logger.Start();
            logger.CursorVisible = false;
            logger.LogLevel = LogSeverity.Verbose;

            await _services.GetService<ICommandService>()
                .InstallCommandsAsync(Assembly.GetExecutingAssembly(), _services);

            await _services.GetService<MrConnectConnector>().StartAsync();

            //Init server
            _services.GetService<WoTServerService>().Init(
                client => new Network.SocketWoTUser(client),
                (cService, user, packet) => new DataServerHelpers.CustomCommandContext(
                    _services.GetService<ICommandService>(),
                    user,
                    packet));

            //Start server
            _services.GetService<WoTServerService>().Start(_services);


            _services.GetService<UpdateService>().Start();

            _services.GetService<UserNotificationService>().Init();

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
