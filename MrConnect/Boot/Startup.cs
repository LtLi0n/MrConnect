using Discord.Commands;
using Discord.WebSocket;
using LionLibrary.Framework;
using LionLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using MrConnect.Services;
using SharedDiscord;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MrConnect.Boot
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
            //sc.AddSingleton<ConnectorWoT>();

            AppConfig config = new AppConfig();
            sc.AddSingleton(config);

            sc.AddSingleton<IDiscordServiceConnectionConfig>(config);
            sc.AddSingleton<DiscordConnector>();

            return sc.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            ILogService logger = _services.GetService<ILogService>();
            logger.Start();
            logger.LogLevel = LogSeverity.Debug;
            logger.CursorVisible = false;

            DiscordService discord = _services.GetService<DiscordService>();
            await discord.InstallCommandsAsync();
            await discord.StartAsync();

            await _services.GetService<DiscordConnector>().StartAsync();
            //await _services.GetService<ConnectorWoT>().StartAsync();

            await Task.Delay(-1);
        }
    }
}
