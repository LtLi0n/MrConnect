using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using SharedDiscord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServerDiscord.Services
{
    public interface IDataContext : IDisposable
    {
        DbSet<User> Users { get; set; }

        Task<U2> AddEntityAsync<U1, U2>(IEntity<U1, U2> obj, DbSet<U1> db_set = null) where U1 : class;
        Task UpdateEntityAsync<U1, U2>(IEntity<U1, U2> entity) where U1 : class;
        Task UpdateEntityAsync<U1, U2>(IEntity<U1, U2> entity, IDictionary<string, object> update_values) where U1 : class;
    }
}