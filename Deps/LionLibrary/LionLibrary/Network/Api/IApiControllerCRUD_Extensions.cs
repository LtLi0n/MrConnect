using LionLibrary.SQL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LionLibrary.Network
{
    ///<summary>Temporary solution for <see href="https://github.com/dotnet/roslyn/issues/39219"/></summary>
    public static class IApiControllerCRUD_Extensions
    {
        public static async Task<IEnumerable<T>> GetAsync<T, IEntityT, PkT>(this IApiControllerCRUD<IEntityT, PkT> crud) 
            where T : IEntityT
            where IEntityT : class =>
            (await crud.GetAsync("x => x", null).ConfigureAwait(false)).AsEnumerable<T>();

        public static async Task<IDictionary<PkT, T>> GetAsDictionaryAsync<T, IEntityT, PkT>(this IApiControllerCRUD<IEntityT, PkT> crud)
            where T : IEntityT, IEntity<T, PkT>
            where IEntityT : class
            where PkT : struct
        {
            Dictionary<PkT, T> entitiesDic = new Dictionary<PkT, T>();

            var entitiesPacket = await crud.GetAsync().ConfigureAwait(false);
            if(entitiesPacket.Status != StatusCode.Success)
            {
                return entitiesDic;
            }
            IEnumerable<T> entities = entitiesPacket.AsEnumerable<T>();

            foreach (T entity in entities)
            {
                entitiesDic.Add(entity.Id, entity);
            }

            return entitiesDic;
        }

        public static async Task<IDictionary<PkT, T>> GetAsDictionaryAsync<T, IEntityT, PkT>(this IApiControllerCRUD<IEntityT, PkT> crud, Func<T, PkT> keyExtractFunc)
            where T : IEntityT, IEntity<T, PkT>
            where IEntityT : class
            where PkT : struct
        {
            Dictionary<PkT, T> entitiesDic = new Dictionary<PkT, T>();

            var entitiesPacket = await crud.GetAsync().ConfigureAwait(false);
            if (entitiesPacket.Status != StatusCode.Success)
            {
                return entitiesDic;
            }
            IEnumerable<T> entities = entitiesPacket.AsEnumerable<T>();

            foreach (T entity in entities)
            {
                entitiesDic.Add(keyExtractFunc(entity), entity);
            }

            return entitiesDic;
        }
    }
}
