using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LionLibrary.Commands;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LionLibrary.SQL
{
    public static class Extensions
    {
        ///<summary>Adds an object to the database.</summary>
        public static async Task<T2> AddEntityAsync<T1, T2>(this DbContext context, IEntity<T1, T2> entity, DbSet<T1> db_set = null)
            where T1 : class
        {
            EntityEntry<T1> result = await (db_set != null ? db_set.AddAsync((T1)entity) : context.AddAsync((T1)entity));
            try
            {
                await context.SaveChangesAsync();
                return (result.Entity as IEntity<T1, T2>).Id;
            }
            catch (Exception ex)
            {
                context.Remove(entity);
                throw new ManagedCommandException(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        ///<summary>Adds an object to the database.</summary>
        public static async Task<T> AddEntityAsync<T>(this DbContext context, IEntityBase<T> entity, DbSet<T> db_set = null)
            where T : class
        {
            EntityEntry<T> result = await (db_set != null ? db_set.AddAsync((T)entity) : context.AddAsync((T)entity));
            try
            {
                await context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                context.Remove(entity);
                throw new ManagedCommandException(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        #region entity
        public static async Task UpdateEntityAsync<T1, T2>(this DbContext context, IEntity<T1, T2> entity)
            where T1 : class => await UpdateEntityFinalAsync(context, entity);

        public static async Task UpdateEntityAsync<T>(this DbContext context, IEntityBase<T> entity)
            where T : class => await UpdateEntityFinalAsync(context, entity);

        public static async Task UpdateEntityAsync<T1, T2>(this DbContext context, IEntity<T1, T2> entity, IDictionary<string, object> update_values)
            where T1 : class
        {
            context.Entry(entity).CurrentValues.SetValues(update_values);
            await AttemptSaveChangesAsync(context);
        }
        #endregion

        private static async Task UpdateEntityFinalAsync<T>(this DbContext context, T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
            await AttemptSaveChangesAsync(context);
        }

        private static async Task AttemptSaveChangesAsync(this DbContext context)
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ManagedCommandException(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }
    }
}
