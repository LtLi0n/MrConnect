using Discord.Commands;
using Discord.WebSocket;
using LionLibrary.Framework;
using LionLibrary.Utils;
using Microsoft.Extensions.DependencyInjection;
using MrConnect.Services;
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
            ContainerConfig.RegisterTypes(sc);
            sc.AddSingleton<IAppConfig, AppConfig>();
            sc.AddSingleton<IDiscordService, DiscordService>();
            sc.AddSingleton<IWoTService, WoTService>();

            return sc.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            ILogService logger = _services.GetService<ILogService>();
            logger.Start();
            logger.CursorVisible = false;

            IDiscordService discord = _services.GetService<IDiscordService>();
            await discord.InstallCommandsAsync();
            await discord.StartAsync();

            await _services.GetService<IWoTService>().StartAsync();

            await Task.Delay(-1);
        }
    }
}
