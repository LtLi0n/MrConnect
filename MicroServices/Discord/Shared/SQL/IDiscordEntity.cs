using LionLibrary.SQL;
using System;

namespace Discord.Shared
{
    public interface IDiscordEntity<EntityT, KeyT> : IEntity<EntityT, KeyT>
    {
        DateTime LastUpdatedAt { get; set; }
        DateTime AddedAt { get; set; }
    }
}
