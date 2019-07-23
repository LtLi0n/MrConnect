using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace MrConnect.Services
{
    public interface IDiscordService
    {
        DiscordSocketClient Client { get; }
        CommandService Commands { get; }

        Task InstallCommandsAsync();
        Task StartAsync();
    }
}