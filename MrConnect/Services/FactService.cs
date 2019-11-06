using System;
using System.Threading.Tasks;
using System.Timers;
using Discord.Shared;
using LionLibrary.Framework;

namespace MrConnect.Server
{
    public class FactService
    {
        private readonly ILogService _logger;
        private readonly Timer _timer = new Timer(60000);
        
        public DiscordService Discord { get; }

        public DiscordConnector DiscordConnector { get; }
        public FactApi FactApi => DiscordConnector.GetController<FactApi>();
        public UserApi UserApi => DiscordConnector.GetController<UserApi>();

        public FactService(DiscordService discord, DiscordConnector discordConn, ILogService logger = null)
        {
            Discord = discord;
            DiscordConnector = discordConn;
            _logger = logger;

            _timer.Elapsed += Timer_Elapsed;
        }

        public async Task StartAsync()
        {
            _timer.Start();
            await TrySetRandomFact();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await TrySetRandomFact();
        }

        private async Task<bool> TrySetRandomFact()
        {
            try
            {
                Fact fact = await FactApi.GetRandomFactAsync();

                if (fact != null)
                {
                    User user = await UserApi.GetUserAsync(fact.UserId);
                    if(user != null)
                    {
                        await Discord.Client.SetGameAsync($"\"{fact.Content}\" - {user.Username}#{user.Discriminator}.");
                        return true;
                    }
                }
            } 
            catch (Exception ex)
            {
                _logger?.LogLine(this, ex.Message, LogSeverity.Error);
            }

            return false;
        }
    }
}
