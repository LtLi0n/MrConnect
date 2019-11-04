using LionLibrary.Network;

using static DataServerHelpers.SharedRef;

namespace Discord.Shared
{
    public static class ApiControllerBucketExtensions
    {
        public static void FillDiscordEntity<EntityT, PkType>(
            this PacketBuilder pb, 
            IDiscordEntity<EntityT, PkType> entity)
            where EntityT : IDiscordEntity<EntityT, PkType>
        {
            pb[Id] = entity.Id;
            pb[AddedAt] = entity.AddedAt;
            pb[LastUpdatedAt] = entity.LastUpdatedAt;
        }
    }
}
