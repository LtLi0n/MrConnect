using System;
using System.Threading.Tasks;

namespace LionLibrary.Network
{
    public static class CRUD
    {
        ///<summary>Attempts to add an entity.</summary>
        ///<typeparam name="T">Entity Type.</typeparam>
        ///<param name="header">Request path.</param>
        ///<param name="entity">Entity instance. Note: you should never change the id value when modifying.</param>
        ///<param name="packet_filler">Method that fills each entity property value into the packetbuilder.</param>
        public static Task<Packet> AddEntityAsync<T>(this LionClient client, string header, T entity, Action<PacketBuilder, T> packet_filler)
            where T : class =>
            client.DownloadPacketAsync(x =>
            {
                x.Header = header;
                packet_filler(x, entity);
            });

        ///<summary>Attempts to retrieve a single entity by its id.</summary>
        ///<typeparam name="T">Entity Type.</typeparam>
        ///<param name="header">Request path.</param>
        ///<param name="id_tag">Host specified syntax for id arguments.</param>
        ///<param name="id">id of entity to retrieve.</param>
        public static Task<Packet<T>> GetEntityAsync<T>(this LionClient client, string header, string id_tag, object id) 
            where T : class =>
            client.DownloadPacketAsync<T>(x =>
            {
                x.Header = header;
                x[id_tag] = id;
            });

        ///<summary>Attempts to retrieve a single entity by its id.</summary>
        ///<typeparam name="T">Entity Type.</typeparam>
        ///<param name="header">Request path.</param>
        ///<param name="id_tag">Host specified syntax for id arguments.</param>
        ///<param name="id">id of entity to retrieve.</param>
        public static Task<Packet<T>> GetEntityAsync<T>(this LionClient client, string header, string id_tag, object id, string where_tag, string select_tag)
            where T : class =>
            client.DownloadPacketAsync<T>(x =>
            {
                x.Header = header;
                x[where_tag] = $"x => x.{id_tag} == {id}";
                x[select_tag] = "x => x";
            });

        ///<summary>Attempts to retrieve a collection of entity keys.</summary>
        ///<typeparam name="PK_T">Entity primary key type.</typeparam>
        ///<param name="header">Request path.</param>
        public static Task<Packet<PK_T>> GetEntitiesAsync<PK_T>(this LionClient client, string header) =>
            client.DownloadPacketAsync<PK_T>(x =>
            {
                x.Header = header;
            });

        ///<summary>Attempts to retrieve a collection of entities.</summary>
        ///<param name="header">Request path.</param>
        [Obsolete("Use generic overload instead.")]
        public static Task<Packet> GetEntitiesAsync(
            this LionClient client,
            string header,
            string selectTag,
            string selector,
            string whereTag,
            string where) =>
            client.DownloadPacketAsync(x =>
            {
                x.Header = header;
                x[selectTag] = selector;

                if (!string.IsNullOrEmpty(where))
                {
                    x[whereTag] = where;
                }
            });

        ///<summary>Attempts to retrieve a collection of entities.</summary>
        ///<param name="header">Request path.</param>
        public static Task<Packet<EntityT>> GetEntitiesAsync<EntityT>(
            this LionClient client,
            string header,
            string selectTag,
            string selector,
            string whereTag,
            string where) =>
            client.DownloadPacketAsync<EntityT>(x =>
            {
                x.Header = header;
                x[selectTag] = selector;

                if (!string.IsNullOrEmpty(where))
                {
                    x[whereTag] = where;
                }
            });

        ///<summary>Attempts to retrieve a collection of requested entities.</summary>
        ///<typeparam name="T">Entity Type.</typeparam>
        ///<param name="header">Request path.</param>
        ///<param name="id_tag">Host specified syntax for id arguments.</param>
        ///<param name="ids">Array of ids of entities to retrieve.</param>
        public static Task<Packet<T>> GetEntitiesAsync<T, PkT>(this LionClient client, string header, string id_tag, PkT[] ids)
            where T : class =>
            client.DownloadPacketAsync<T>(x =>
            {
                x.Header = header;
                x[id_tag] = string.Join(',', ids);
            });

        ///<summary>Attempts to modify an entity.</summary>
        ///<typeparam name="T">Entity Type.</typeparam>
        ///<param name="header">Request path.</param>
        ///<param name="entity">Entity instance. Note: you should never change the id value when modifying.</param>
        ///<param name="packet_filler">Method that fills each entity property value into the packetbuilder.</param>
        public static Task<Packet> ModifyEntityAsync<T>(this LionClient client, string header, T entity, Action<PacketBuilder, T> packet_filler)
            where T : class => AddEntityAsync(client, header, entity, packet_filler);

        ///<summary>Attempts to remove an entity.</summary>
        ///<param name="header">Request path.</param>
        ///<param name="entity_id">Entity id that is to be removed.</param>
        ///<param name="id_tag">Host specified syntax for id arguments.</param>
        public static Task<Packet> RemoveEntityAsync(this LionClient client, string header, string id_tag, object entity_id) =>
            client.DownloadPacketAsync(x =>
            {
                x.Header = header;
                x[id_tag] = entity_id;
            });
    }
}
