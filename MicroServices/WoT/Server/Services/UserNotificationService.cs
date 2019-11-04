using MrConnect.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using WoT.Shared;
using System.Collections.Generic;
using Discord;

namespace WoT.Server
{
    public class UserNotificationService
    {
        private readonly IServiceProvider _services;
        public WoTDbContext Db => _services.GetService<WoTDbContext>();

        public MrConnectConnector MrConnect { get; }
        public MrConnect.Shared.UserApi UserApi => MrConnect.GetController<MrConnect.Shared.UserApi>();

        public CharacterWorkService CharacterWorkService { get; }

        public UserNotificationService(IServiceProvider services, MrConnectConnector mrConnect, CharacterWorkService workService)
        {
            _services = services;
            MrConnect = mrConnect;
            CharacterWorkService = workService;
        }

        public void Init()
        {
            CharacterWorkService.WorkFinished += (o, e) =>
            {
                User user = Db.Set<User>().Find(e.Character.UserId);
                CharacterWork worker = e.Entity as CharacterWork;

                UserApi.SendMessage(
                    user.DiscordId,
                    embed: new SharedDiscordEmbed
                    {
                        Color = new Color(25, 78, 143).RawValue,
                        Fields = new List<SharedDiscordEmbedField>
                        {
                            new SharedDiscordEmbedField
                            {
                                Name = $"Mining Completed",
                                Value = 
                                $"```swift\n" +
                                $"You Were Mining For {e.Hours} Hours\n" +
                                $"Gold Obtained: +{e.Reward} ({e.Character.Gold})```"
                            },
                            worker.AsEmbedField()
                        }
                    });
            };
        }
    }
}
