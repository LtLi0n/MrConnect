using LionLibrary.Network;

using static DataServerHelpers.SharedRef;

namespace DataServerHelpers
{
    public abstract class EpicApiController<EntityT, KeyType> : ApiController, IApiControllerCRUD<EntityT, KeyType>
        where EntityT : class
    {
        public string IdTag => Id;
        public string SelectTag => Select;
        public string WhereTag => Where;

        public string FullModulePath { get; }

        public string AddRoute { get; }
        public string GetRoute { get; }
        public string ModifyRoute { get; }
        public string RemoveRoute { get; }

        public IApiControllerCRUD<EntityT, KeyType> CRUD => this;

        public EpicApiController(ServerConnector conn, string fullModulePath) : base(conn)
        {
            FullModulePath = fullModulePath;

            AddRoute = $"{fullModulePath}.add";
            GetRoute = $"{fullModulePath}.get";
            ModifyRoute = $"{fullModulePath}.modify";
            RemoveRoute = $"{fullModulePath}.remove";
        }

        public abstract void FillPacket(PacketBuilder pb, EntityT entity);
    }
}
