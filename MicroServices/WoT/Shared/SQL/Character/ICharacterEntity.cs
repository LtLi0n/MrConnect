using LionLibrary.SQL;

namespace WoT.Shared
{
    public interface ICharacterEntity<T> : IEntityBase<T>
    {
        uint CharacterId { get; }
    }
}
