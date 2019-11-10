using Microsoft.EntityFrameworkCore;

namespace LionLibrary.SQL
{
    public static class ContextExtensions
    {
        public static string GetTableName<T>(this DbContext context) where T : class
        {
            var mapping = context.Model.FindEntityType(typeof(T)).Relational();
            return mapping.TableName;
        }
    }
}
