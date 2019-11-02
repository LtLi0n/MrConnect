using LionLibrary.Commands;
using LionLibrary.SQL;
using LionLibrary.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataServerHelpers
{
    public static class SocketDataModuleBaseExtensions
    {
        public static async Task WrapperGetEntitiesAsync<EntityT>(
            this SocketDataModuleBase<CustomCommandContext, DbContext> module)
            where EntityT : class, IEntityBase<EntityT>
        {
            //await ValidateCallerAsync();
            //await ValidatePermissionAsync<EntityT>(Permission.Read);

            module.ReplyEntries(module.SQL.Set<EntityT>());
        }

        public static async Task WrapperGetEntitiesAsync<EntityT>(
            this SocketDataModuleBase<CustomCommandContext, DbContext> module,
            Expression<Func<EntityT, object>> defaultSelector)
            where EntityT : class, IEntityBase<EntityT>
        {
            //await ValidateCallerAsync();
            //await ValidatePermissionAsync<EntityT>(Permission.Read);

            if (module.HasArg(SharedRef.Where) || module.HasArg(SharedRef.Select))
            {
                await module.WrapperGetEntitiesAsync<EntityT>();
            }
            else
            {
                module.ReplyFullEntries(module.SQL.Set<EntityT>().Select(defaultSelector));
            }
        }

        public static async Task<bool> WrapperModifyEntityAsync<EntityT, PkType>(
            this SocketDataModuleBase<CustomCommandContext, DbContext> module,
            string idTag = SharedRef.Id)
            where EntityT : class, IEntity<EntityT, PkType>
        {
            if (module is IDataModule<EntityT> dataModule)
            {
                //await ValidateCallerAsync();

                PkType key = default;
                module.TryFill<PkType>(idTag, x => key = x);

                EntityT entity = module.SQL.Set<EntityT>().Find(key);

                if (entity != null)
                {
                    //await ValidateModifyRequestAsync(entity);
                    dataModule.ApplyInput(entity);
                    await module.UpdateEntityAsync(entity);
                    module.ReplyModifySuccess(entity);
                    return true;
                }
                else
                {
                    module.ReplyEntityNotFound<EntityT>(key);
                    return false;
                }
            }
            else
            {
                throw new Exception("");
            }
        }

        public static async Task<bool> WrapperModifyEntityAsync<EntityT>(
            this SocketDataModuleBase<CustomCommandContext, DbContext> module,
            params object[] keys)
            where EntityT : class, IEntityBase<EntityT>
        {
            if(module is IDataModule<EntityT> dataModule)
            {
                //await ValidateCallerAsync();

                EntityT entity = module.SQL.Set<EntityT>().Find(keys);

                if (entity != null)
                {
                    //await ValidateModifyRequestAsync(entity);
                    dataModule.ApplyInput(entity);
                    await module.UpdateEntityAsync(entity);
                    module.ReplyModifySuccess<EntityT>(keys);
                    return true;
                }
                else
                {
                    module.ReplyEntityNotFound<EntityT>(keys);
                    return false;
                }
            }
            else
            {
                throw new Exception("");
            }
        }


        public static async Task<bool> WrapperRemoveEntityAsync<EntityT, PKType>(
            this SocketDataModuleBase<CustomCommandContext, DbContext> module,
            string idTag = SharedRef.Id)
            where EntityT : class, IEntity<EntityT, PKType>
        {
            //await ValidateCallerAsync();

            //primary key value
            PKType key = default;
            module.TryFill<PKType>(idTag, x => key = x);

            EntityT entity = module.SQL.Set<EntityT>().Find(key);

            if (entity != null)
            {
                //await ValidatePermissionAsync<EntityT>(entity.PridėjoId == Context.User.Id ? Permission.Create : Permission.Manage);
                await module.RemoveEntityAsync(entity);
                module.ReplyRemoveSuccess(entity);
                return true;
            }
            else
            {
                module.ReplyEntityNotFound<EntityT, PKType>(key);
                return false;
            }
        }

        public static async Task<bool> WrapperRemoveEntityAsync<EntityT>(
            this SocketDataModuleBase<CustomCommandContext, DbContext> module,
            params object[] keys)
            where EntityT : class, IEntityBase<EntityT>
        {
            //await ValidateCallerAsync();

            EntityT entity = module.SQL.Set<EntityT>().Find(keys);

            if (entity != null)
            {
                //await ValidatePermissionAsync<EntityT>(entity.PridėjoId == Context.User.Id ? Permission.Create : Permission.Manage);
                await module.RemoveEntityAsync(entity);
                module.ReplyRemoveSuccess<EntityT>(keys);
                return true;
            }
            else
            {
                module.ReplyEntityNotFound<EntityT>(keys);
                return false;
            }
        }
    }
}
