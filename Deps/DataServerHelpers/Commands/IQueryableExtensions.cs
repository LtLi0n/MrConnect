using System.Linq;
using System.Linq.Dynamic.Core;
using LionLibrary.Commands;

using static DataServerHelpers.SharedRef;

namespace DataServerHelpers
{
    public static class IQueryableExtensions
    {
        ///<summary>Dynamic could mean worse performance, but flexibility tradeoff is worth it.</summary>
        public static IQueryable Select(this IQueryable query, SocketCommandContext context)
        {
            if (context.Args.ContainsKey(SharedRef.Select))
            {
                var queryString = context.Args[SharedRef.Select];

                return query.Select(queryString);
            }
            else
            {
                return query.Select($"x => x.{Id}");
            }
        }

        ///<summary>Dynamic could mean worse performance, but flexibility tradeoff is worth it.</summary>
        public static IQueryable Where(this IQueryable query, SocketCommandContext context)
        {
            if (context.Args.ContainsKey(SharedRef.Where))
            {
                var queryString = context.Args[SharedRef.Where];

                return query.Where(queryString);
            }

            return query;
        }
    }
}
