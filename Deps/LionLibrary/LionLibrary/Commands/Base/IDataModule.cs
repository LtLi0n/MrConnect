using LionLibrary.SQL;
using System.Threading.Tasks;

namespace LionLibrary.Commands
{
    ///<summary>Use it to not forget mandatory methods for back-end data filling.</summary>
    ///<typeparam name="T">Data object.</typeparam>
    public interface IDataModule<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="assign_mandatory">An option to skip mandatory variable assign logic when they were assigned upon initial object construction.</param>
        void ApplyInput(T entity, bool assign_mandatory = true);

        Task AddAsync();
        Task GetAsync();
        Task ModifyAsync();
        Task RemoveAsync();
    }
}
