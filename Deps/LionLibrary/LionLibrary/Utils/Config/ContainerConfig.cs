using System;
using LionLibrary.Commands;
using LionLibrary.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace LionLibrary.Utils
{
    public static class ContainerConfig
    {
        public static void RegisterLionLibraryTypes(this IServiceCollection collection)
        {
            collection.AddSingleton<ILogService, LogService>();
            collection.AddSingleton<ICommandService, CommandService>();
        }
    }
}
