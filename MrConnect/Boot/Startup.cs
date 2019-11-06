using Discord.Commands;
using Discord.WebSocket;
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
using WoT.Shared;
using Discord.Shared;

namespace MrConnect.Server.Boot
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
            sc.AddSingleton<DiscordService>();
            sc.AddSingleton<WoTConnector>();
            sc.AddSingleton<DiscordConnector>();

            AppConfig config = new AppConfig();
            {
                sc.AddSingleton(config);
                sc.AddSingleton(config.WoTServiceConnectionConfig);
                sc.AddSingleton(config.DiscordServiceConnectionConfig);
                sc.AddSingleton<IServerConfig>(config);
                sc.AddSingleton<ISslServerConfig>(config);
                sc.AddSingleton<IDataModuleConfig>(config);
            }

            sc.AddSingleton<MrConnectServerService>();
            sc.AddSingleton<FactService>();

            return sc.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            ILogService logger = _services.GetService<ILogService>();
            logger.Start();
            logger.LogLevel = LogSeverity.Debug;
            logger.CursorVisible = false;

            await _services.GetService<ICommandService>()
                .InstallCommandsAsync(Assembly.GetExecutingAssembly(), _services);

            //Init server
            _services.GetService<MrConnectServerService>().Init(
                client => new Network.SocketMrConnectUser(client),
                (cService, user, packet) => new DataServerHelpers.CustomCommandContext(
                    _services.GetService<ICommandService>(),
                    user,
                    packet));

            //Start server
            _services.GetService<MrConnectServerService>().Start(_services);

            DiscordService discord = _services.GetService<DiscordService>();
            await discord.InstallCommandsAsync();
            await discord.StartAsync();

            await _services
                .GetService<DiscordConnector>()
                .StartAsync()
                .ContinueWith(async x => await _services.GetService<FactService>().StartAsync());


            _services.GetService<WoTConnector>().StartAsync();

            await Task.Delay(-1);
        }
    }
}
