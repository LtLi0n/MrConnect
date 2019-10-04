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
using WoT.Server.Services;

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
            
            AppConfig config = new AppConfig();
            {
                sc.AddSingleton(config);
                sc.AddSingleton<IServerConfig>(config);
                sc.AddSingleton<ISslServerConfig>(config);
                sc.AddSingleton<IDataModuleConfig>(config);
            }

            sc.AddSingleton<WoTServer>();
            sc.AddDbContext<WoTDbContext>(x => WoTDbContext.UseMySqlOptions(x, config), contextLifetime: ServiceLifetime.Transient);

            return sc.BuildServiceProvider();
        }

        public async Task StartAsync()
        {
            ILogService logger = _services.GetService<ILogService>();
            logger.Start();
            logger.CursorVisible = false;

            await _services.GetService<ICommandService>()
                .InstallCommandsAsync(Assembly.GetExecutingAssembly(), _services);
            
            _services.GetService<WoTServer>().Start(_services);

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
