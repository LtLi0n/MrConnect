using System;
using System.Threading.Tasks;

namespace LionLibrary.Network
{
    public static class CRUD_Owned
    {
        ///<summary>Attempts to add an owned entity.</summary>
        public static async Task<Packet> AddOwnedEntityAsync<T>(
            this LionClient client,
            string header,
            T entity,
            Action<PacketBuilder, T> packet_filler,
            string id_tag,
            object parentId)
        {
            return await client.DownloadPacketAsync(x =>
            {
                x.Header = header;
                x[id_tag] = parentId;
                packet_filler(x, entity);
            });
        }

        ///<summary>Attempts to get an owned entity.</summary>
        public static async Task<Packet<T>> GetOwnedEntityAsync<T>(
            this LionClient client,
            string header,
            string id_tag,
            object parentId) where T : class
        {
            return await client.DownloadPacketAsync<T>(x =>
            {
                x.Header = header;
                x[id_tag] = parentId;
            });
        }

        ///<summary>Attempts to modify an owned entity.</summary>
        public static async Task<Packet> ModifyOwnedEntityAsync<T>(
            this LionClient client,
            string header,
            T entity,
            Action<PacketBuilder, T> packet_filler,
            string id_tag,
            object parentId) => 
            await AddOwnedEntityAsync(client, header, entity, packet_filler, id_tag, parentId);

        ///<summary>Attempts to remove an owned entity.</summary>
        public static async Task<Packet> RemoveOwnedEntityAsync<PK_T>(
            this LionClient client,
            string header,
            string id_tag,
            PK_T parentId)
        {
            return await client.DownloadPacketAsync(x =>
            {
                x.Header = header;
                x[id_tag] = parentId;
            });
        }
    }
}
