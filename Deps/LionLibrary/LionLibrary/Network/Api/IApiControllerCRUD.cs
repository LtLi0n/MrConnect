using LionLibrary.SQL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionLibrary.Network
{
    public interface IApiControllerCRUD<IEntityT, PkT> 
        where IEntityT : class
    {
        string IdTag { get; }
        string SelectTag { get; }
        string WhereTag { get; }

        string AddRoute { get; }
        string GetRoute { get; }
        string ModifyRoute { get; }
        string RemoveRoute { get; }

        LionClient Client { get; }
        
        void FillPacket(PacketBuilder pb, IEntityT entity);

        Task<Packet> AddAsync(IEntityT entity) => 
            Client.AddEntityAsync(AddRoute, entity, FillPacket);

        Task<Packet<IEntityT>> GetAsync(PkT id) =>
            Client.GetEntityAsync<IEntityT>(GetRoute, IdTag, id);

        Task<Packet<IEntityT>> GetWhereAsync(PkT id) =>
            Client.GetEntityAsync<IEntityT>(GetRoute, IdTag, id, WhereTag, SelectTag);

        Task<Packet<IEntityT>> GetAsync(PkT[] ids) =>
            Client.GetEntitiesAsync<IEntityT, PkT>(GetRoute, IdTag, ids);

        Task<Packet<IEntityT>> GetAsync(string select = "x => x", string where = null) =>
            Client.GetEntitiesAsync<IEntityT>(GetRoute, SelectTag, select, WhereTag, where);

        ///<summary>Use <see cref="IApiControllerCRUD_Extensions.GetAsync{T, IEntityT, PkT}(IApiControllerCRUD{IEntityT, PkT})"/> instead.</summary>
        [Obsolete("Needs a fix from microsoft - https://github.com/dotnet/roslyn/issues/39219.")]
        async Task<IEnumerable<T>> GetAsync<T>() where T : IEntityT
        {
            Packet<IEntityT> packet = await GetAsync("x => x", null).ConfigureAwait(false);
            return packet.AsEnumerable<T>();
        }

        ///<summary>Use <see cref="IApiControllerCRUD_Extensions.GetAsDictionaryAsync{T, IEntityT, PkT}(IApiControllerCRUD{IEntityT, PkT})"/> instead.</summary>
        [Obsolete("Needs a fix from microsoft - https://github.com/dotnet/roslyn/issues/39219.")]
        async Task<IDictionary<PkT, T>> GetAsDictionaryAsync<T>() 
            where T : IEntityT, IEntity<T, PkT>
        {
            Dictionary<PkT, T> entitiesDic = new Dictionary<PkT, T>();

            var entitiesPacket = await GetAsync().ConfigureAwait(false);
            IEnumerable<T> entities = entitiesPacket.AsEnumerable<T>();

            foreach (T entity in entities)
            {
                entitiesDic.Add(entity.Id, entity);
            }

            return entitiesDic;
        }

        ///<summary>Use <see cref="IApiControllerCRUD_Extensions.GetAsDictionaryAsync{T, IEntityT, PkT}(IApiControllerCRUD{IEntityT, PkT}, Func{T, PkT})"/> instead.</summary>
        [Obsolete("Needs a fix from microsoft - https://github.com/dotnet/roslyn/issues/39219.")]
        async Task<IDictionary<PkT, T>> GetAsDictionaryAsync<T>(Func<T, PkT> keyExtractFunc)
            where T : IEntityT, IEntityBase<T>
        {
            Dictionary<PkT, T> entitiesDic = new Dictionary<PkT, T>();

            var entitiesPacket = await GetAsync().ConfigureAwait(false);
            IEnumerable<T> entities = entitiesPacket.AsEnumerable<T>();

            foreach (T entity in entities)
            {
                entitiesDic.Add(keyExtractFunc(entity), entity);
            }

            return entitiesDic;
        }

        Task<Packet> ModifyAsync(IEntityT entity) =>
            Client.ModifyEntityAsync(ModifyRoute, entity, FillPacket);

        Task<Packet> RemoveAsync(PkT entity_id) =>
            Client.RemoveEntityAsync(RemoveRoute, IdTag, entity_id);
    }
}
