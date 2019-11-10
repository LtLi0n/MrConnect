using System.Threading.Tasks;

namespace LionLibrary.Network
{
    public interface IApiControllerCRUD_Owned<EntityT, PkT>
        where EntityT : class
    {
        void FillPacket(PacketBuilder pb, EntityT entity);

        Task<Packet> AddAsync(EntityT entity, PkT parentId);
        Task<Packet<EntityT>> GetAsync(PkT parentId);
        Task<Packet> ModifyAsync(EntityT entity, PkT parentId);
        Task<Packet> RemoveAsync(PkT parentId);
    }
}
