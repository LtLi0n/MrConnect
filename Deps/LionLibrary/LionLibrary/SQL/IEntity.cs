namespace LionLibrary.SQL
{
    ///<summary>EntityFramework helper base.</summary>
    ///<typeparam name="T1">Data Type.</typeparam>
    ///<typeparam name="T2">Indexer.</typeparam>
    public interface IEntity<T1, T2> : IEntityBase<T1>
    {
        T2 Id { get; set;  }
    }

    public interface IEntityBase<T> { }
}
