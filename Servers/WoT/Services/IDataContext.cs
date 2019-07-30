using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerWoT.Services
{
    public interface IDataContext : IDisposable
    {
        Task<U2> AddEntityAsync<U1, U2>(IEntity<U1, U2> obj, DbSet<U1> db_set = null)
            where U1 : class 
            where U2 : struct;

        Task UpdateEntityAsync<U1, U2>(IEntity<U1, U2> entity) where U2 : struct;
        Task UpdateEntityAsync<U1, U2>(IEntity<U1, U2> entity, IDictionary<string, object> update_values)
            where U2 : struct;
    }
}