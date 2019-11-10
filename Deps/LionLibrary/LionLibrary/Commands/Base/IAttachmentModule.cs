using LionLibrary.SQL;
using System.Threading.Tasks;

namespace LionLibrary.Commands
{
    ///<summary>Use it to signal the object is an owner of a file storage.</summary>
    ///<typeparam name="T">Data object.</typeparam>
    public interface IAttachmentModule<T> where T : IEntityBase<T>
    {
        ///<summary>Common arguments are: 'file_name'</summary>
        Task AddAttachmentAsync();
        
        ///<summary>Common arguments are: 'file_name'</summary>
        Task GetAttachmentAsync();

        ///<summary>Common arguments are: none</summary>
        Task GetAttachmentEntriesAsync();

        ///<summary>Common arguments are: 'file_name'</summary>
        Task RemoveAttachmentAsync();
    }
}
