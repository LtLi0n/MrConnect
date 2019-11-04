using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using LionLibrary.Utils;
using LionLibrary.Commands;

namespace DataServerHelpers
{
    public class SocketDataModuleBase<ContextT, TDbContext> : SocketModuleBase<ContextT>
        where ContextT : CustomCommandContext
        where TDbContext : DbContext
    {
        public TDbContext SQL { get; set; }
        public IDataModuleConfig Config { get; set; }
        public DbContext DbContext => SQL as DbContext;

        ///<summary>Adds an object to the database. Note: This forces <see cref="SaveChangesAsync"/> method to obtain the id of a newly created entity.</summary>
        ///<typeparam name="PKType">Entity type.</typeparam>
        ///<typeparam name="PKType">Entity primary key type.</typeparam>
        ///<param name="entity">An entity instance to add to the database.</param>
        ///<returns>Primary key of a newly created entity.></returns>
        public async Task<PKType> AddEntityAsync<EntityT, PKType>(IEntity<EntityT, PKType> entity)
            where EntityT : class, IEntity<EntityT, PKType>
            where PKType : struct, IComparable, IComparable<PKType>, IConvertible, IEquatable<PKType>, IFormattable =>
            await DbContext.AddEntityAsync(entity);

        public async Task AddEntityAsync<EntityT>(IEntityBase<EntityT> entity)
            where EntityT : class =>
            await DbContext.AddEntityAsync(entity);

        public async Task UpdateEntityAsync<EntityT, PKType>(IEntity<EntityT, PKType> entity)
            where EntityT : class, IEntity<EntityT, PKType>
            where PKType : struct, IComparable, IComparable<PKType>, IConvertible, IEquatable<PKType>, IFormattable =>
            await DbContext.UpdateEntityAsync(entity);

        public async Task UpdateEntityAsync<EntityT>(IEntityBase<EntityT> entity)
            where EntityT : class =>
            await DbContext.UpdateEntityAsync(entity);

        public IEnumerable<EntityT> GetEntities<EntityT, PKType>(IQueryable<EntityT> dbset, PKType[] pkArr)
            where EntityT : class, IEntity<EntityT, PKType>
            where PKType : struct, IComparable, IComparable<PKType>, IConvertible, IEquatable<PKType>, IFormattable =>
            dbset.Where(x => pkArr.Contains(x.Id));

        ///<summary>Removes an entity from the database.</summary>
        /// <typeparam name="EntityT">Entity type.</typeparam>
        /// <typeparam name="PKType">Entity primary key type.</typeparam>
        /// <param name="entity">An entity instance to remove from the database.</param>
        public async Task RemoveEntityAsync<EntityT, PKType>(IEntity<EntityT, PKType> entity)
            where EntityT : class, IEntity<EntityT, PKType>
            where PKType : struct, IComparable, IComparable<PKType>, IConvertible, IEquatable<PKType>, IFormattable
        {
            DbContext.Remove(entity);
            await SaveChangesAsync();
        }

        public async Task RemoveEntityAsync<EntityT>(IEntityBase<EntityT> entity)
            where EntityT : class
        {
            DbContext.Remove(entity);
            await SaveChangesAsync();
        }

        ///<summary>Asynchronously saves all changes made in this context to the database.</summary>
        /// <returns></returns>
        public async Task<int> SaveChangesAsync() => await DbContext.SaveChangesAsync();

        ///<summary>Return this for get requests, whenever no arguments are provided.</summary>
        ///<typeparam name="T">Entity type.</typeparam>
        ///<typeparam name="TResult">Selector result.</typeparam>
        ///<param name="set">Entity Set. Can pass DbSet&lt;<typeparamref name="T"/>&gt; because it inherits from IQuerable&lt;<typeparamref name="T"/>&gt;.</param>
        ///<param name="selector">Optional query format. If null, a whole object is retrieved.</param>
        public void ReplyFullEntries<T>(IQueryable<T> set)
            where T : class =>
            Reply(set
                .Skip(GetArgInt32("page") * Config.MaxEntriesPerPage)
                .Take(Config.MaxEntriesPerPage));

        ///<summary>Return this for get requests, whenever no arguments are provided.</summary>
        ///<typeparam name="T">Entity type.</typeparam>
        ///<typeparam name="TResult">Selector result.</typeparam>
        ///<param name="set">Entity Set. Can pass DbSet&lt;<typeparamref name="T"/>&gt; because it inherits from IQuerable&lt;<typeparamref name="T"/>&gt;.</param>
        ///<param name="selector">Query format.</param>
        public void ReplyEntries<T, TResult>(IQueryable<T> set, Expression<Func<T, TResult>> selector)
            where T : class =>
            Reply(set
                .Skip(GetArgInt32(SharedRef.Page) * Config.MaxEntriesPerPage)
                .Take(Config.MaxEntriesPerPage)
                .Select(selector));

        ///<typeparam name="T">Entity type.</typeparam>
        ///<param name="set">Entity Set. Can pass DbSet&lt;<typeparamref name="T"/>&gt; because it inherits from IQuerable&lt;<typeparamref name="T"/>&gt;.</param>
        ///<param name="dynamicSelector">Query format.</param>
        public void ReplyEntries<T>(IQueryable<T> set) =>
            Reply(set
                .Skip(GetArgInt32(SharedRef.Page) * Config.MaxEntriesPerPage)
                .Take(Config.MaxEntriesPerPage)
                .Where(base.Context)
                .Select(base.Context));

        public void ReplyEntries<EntityT, PKType>(IQueryable<EntityT> db_set, PKType[] pkArr)
            where EntityT : class, IEntity<EntityT, PKType>
            where PKType : struct, IComparable, IComparable<PKType>, IConvertible, IEquatable<PKType>, IFormattable
        {
            if (pkArr.Length > Config.MaxEntriesPerPage)
            {
                ReplyError("data request is larger than the limit.");
            }
            else
            {
                Reply(GetEntities(db_set, pkArr));
            }
        }

        public async Task<EntityT> GetEntityByStringIdAsync<EntityT, PKType>(DbSet<EntityT> db_set)
            where EntityT : class, IEntity<EntityT, PKType>
            where PKType : struct, IComparable, IComparable<PKType>, IConvertible, IEquatable<PKType>, IFormattable
        {
            PKType key = default;
            TryFill<PKType>("id", x => key = x);

            EntityT entity = await db_set.FindAsync(key);
            return entity;
        }

        #region Wrappers

        public async Task WrapperAddEntityAsync<EntityT, PkType>(
            Func<EntityT> createFunc,
            bool asignMandatoryThroughApplyInput = false)
            where EntityT : class, IEntity<EntityT, PkType>
        {
            //await ValidateCallerAsync();
            //await ValidatePermissionAsync<Paslauga>(Permission.Create);

            if (this is IDataModule<EntityT> dataModule)
            {
                EntityT entity = createFunc();
                dataModule.ApplyInput(entity, asignMandatoryThroughApplyInput);
                await AddEntityAsync(entity);
                ReplyCreateSuccess(entity);
            }
            else
            {
                throw new Exception($"Module must inherit from IDataModule<{typeof(EntityT).FullName}>.");
            }
        }

        public async Task WrapperGetEntitiesAsync<EntityT>(Expression<Func<EntityT, object>> defaultSelector)
            where EntityT : class, IEntityBase<EntityT>
        {
            //await ValidateCallerAsync();
            //await ValidatePermissionAsync<EntityT>(Permission.Read);

            if (HasArg(SharedRef.Where) || HasArg(SharedRef.Select))
            {
                await WrapperGetEntitiesAsync<EntityT>();
            }
            else
            {
                ReplyFullEntries(SQL.Set<EntityT>().Select(defaultSelector));
            }
        }

        public async Task WrapperGetEntitiesAsync<EntityT>()
            where EntityT : class, IEntityBase<EntityT>
        {
            //await ValidateCallerAsync();
            //await ValidatePermissionAsync<EntityT>(Permission.Read);

            ReplyEntries(SQL.Set<EntityT>());
        }

        public async Task<bool> WrapperModifyEntityAsync<EntityT, PkType>(string idTag = SharedRef.Id)
            where EntityT : class, IEntity<EntityT, PkType>
        {
            if (this is IDataModule<EntityT> dataModule)
            {
                //await ValidateCallerAsync();

                PkType key = default;
                TryFill<PkType>(idTag, x => key = x);

                EntityT entity = SQL.Set<EntityT>().Find(key);

                if (entity != null)
                {
                    //await ValidateModifyRequestAsync(entity);
                    dataModule.ApplyInput(entity);
                    await UpdateEntityAsync(entity);
                    ReplyModifySuccess(entity);
                    return true;
                }
                else
                {
                    ReplyEntityNotFound<EntityT, PkType>(key);
                    return false;
                }
            }
            else
            {
                throw new Exception($"Module must inherit from IDataModule<{typeof(EntityT).FullName}>.");
            }
        }

        public async Task<bool> WrapperModifyEntityAsync<EntityT>(params object[] keys)
            where EntityT : class, IEntityBase<EntityT>
        {
            if (this is IDataModule<EntityT> dataModule)
            {
                //await ValidateCallerAsync();

                EntityT entity = SQL.Set<EntityT>().Find(keys);

                if (entity != null)
                {
                    //await ValidateModifyRequestAsync(entity);
                    dataModule.ApplyInput(entity);
                    await UpdateEntityAsync(entity);
                    ReplyModifySuccess<EntityT>(keys);
                    return true;
                }
                else
                {
                    ReplyEntityNotFound<EntityT>(keys);
                    return false;
                }
            }
            else
            {
                throw new Exception("");
            }
        }


        public async Task<bool> WrapperRemoveEntityAsync<EntityT, PKType>(string idTag = SharedRef.Id)
            where EntityT : class, IEntity<EntityT, PKType>
        {
            //await ValidateCallerAsync();

            //primary key value
            PKType key = default;
            TryFill<PKType>(idTag, x => key = x);

            EntityT entity = SQL.Set<EntityT>().Find(key);

            if (entity != null)
            {
                //await ValidatePermissionAsync<EntityT>(entity.PridėjoId == Context.User.Id ? Permission.Create : Permission.Manage);
                await RemoveEntityAsync(entity);
                ReplyRemoveSuccess(entity);
                return true;
            }
            else
            {
                ReplyEntityNotFound<EntityT, PKType>(key);
                return false;
            }
        }

        public async Task<bool> WrapperRemoveEntityAsync<EntityT>(params object[] keys)
            where EntityT : class, IEntityBase<EntityT>
        {
            //await ValidateCallerAsync();

            EntityT entity = SQL.Set<EntityT>().Find(keys);

            if (entity != null)
            {
                //await ValidatePermissionAsync<EntityT>(entity.PridėjoId == Context.User.Id ? Permission.Create : Permission.Manage);
                await RemoveEntityAsync(entity);
                ReplyRemoveSuccess<EntityT>(keys);
                return true;
            }
            else
            {
                ReplyEntityNotFound<EntityT>(keys);
                return false;
            }
        }

        #endregion

        #region ReplyCreate
        public void ReplyCreateSuccess<EntityT, PKType>(IEntity<EntityT, PKType> entity)
            where EntityT : class =>
            Reply($"[{CreateIdentityString<EntityT>(new object[] { entity.Id })}] successfully added.");

        public void ReplyCreateSuccess<EntityT>(params object[] keys)
            where EntityT : class =>
            Reply($"[{CreateIdentityString<EntityT>(keys)}] successfully added.");
        #endregion
        #region ReplyModify
        public void ReplyModifySuccess<EntityT, PKType>(IEntity<EntityT, PKType> entity)
            where EntityT : class =>
            Reply($"[{CreateIdentityString<EntityT>(new object[] { entity.Id })}] successfully modified.");

        public void ReplyModifySuccess<EntityT>(IEnumerable<object> keys)
            where EntityT : class =>
            Reply($"[{CreateIdentityString<EntityT>(keys)}] successfully modified.");
        #endregion
        #region ReplyRemove
        public void ReplyRemoveSuccess<EntityT, EntityKey>(IEntity<EntityT, EntityKey> entity)
            where EntityT : class =>
            Reply($"[{CreateIdentityString<EntityT>(new object[] { entity.Id })}] successfully removed.");

        public void ReplyRemoveSuccess<EntityT>(object[] keys)
            where EntityT : class =>
            Reply($"[{CreateIdentityString<EntityT>(keys)}] successfully removed.");
        #endregion

        #region ReplyEntityNotFound
        public void ReplyEntityNotFound<EntityT, PkType>(PkType key)
            where EntityT : class, IEntity<EntityT, PkType> =>
            ReplyError($"[{CreateIdentityString<EntityT>(new object[] { key })}] not found.");

        public void ReplyEntityNotFound<EntityT>(IEnumerable<object> keys)
            where EntityT : class =>
            ReplyError($"[{CreateIdentityString<EntityT>(keys)}] not found.");
        #endregion

        private string CreateIdentityString<EntityT>(IEnumerable<object> keys)
            where EntityT : class =>
            $"{SQL.GetTableName<EntityT>()}:{string.Join(':', keys)}";
    }
}
