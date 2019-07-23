using System;
using System.Threading.Tasks;
using LionLibrary.Framework;
using LionLibrary.Commands;
using LionLibrary.Extensions;
using LionLibrary.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using WarsOfTanoth.Boot;

namespace WarsOfTanoth.Services
{
    public class DataContext : DbContext, IDataContext
    {
        private readonly ILogService _logger;
        private readonly IAppConfig _config;

        public DataContext(DbContextOptions<DataContext> options, ILogService logger = null) : base(options)
        {
            _logger = logger;
            Init();
        }

        public DataContext(IAppConfig config, ILogService logger) : base()
        {
            _config = config;
            _logger = logger;
            SyncMysqlOptions(new DbContextOptionsBuilder(), config.ConfigRoot);
            Init();
        }

        private void Init()
        {

        }

        public static void SyncMysqlOptions(DbContextOptionsBuilder optionsBuilder, IConfigurationRoot config)
        {
            optionsBuilder.UseMySql(
                $"server={config["mysql:host"]};" +
                $"database={config["mysql:database"]};" +
                $"user={config["mysql:user"]};" +
                $"password={config["mysql:password"]}");
        }
            

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

            if (!optionsBuilder.IsConfigured)
            {
                if (_config != null) SyncMysqlOptions(optionsBuilder, _config.ConfigRoot);
                else throw new ArgumentNullException("Configuration failed.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);   
            _logger?.LogLine(this, "Entities configured.");
        }

        ///<summary>Adds an object to the database.</summary>
        public async Task<U2> AddEntityAsync<U1, U2>(IEntity<U1, U2> obj, DbSet<U1> db_set = null)
            where U1 : class
            where U2 : struct
        {
            EntityEntry<U1> result = await (db_set != null ? db_set.AddAsync((U1)obj) : AddAsync((U1)obj));
            try
            {
                await SaveChangesAsync();
                return (result.Entity as IEntity<U1, U2>).Id;
            }
            catch (Exception ex)
            {
                Remove(obj);
                throw new ManagedCommandException(ex.InnerException == null ? ex.Message : ex.InnerException.Message);
            }
        }

        public async Task UpdateEntityAsync<U1, U2>(IEntity<U1, U2> entity) 
            where U2 : struct =>
            await Extensions.UpdateEntityAsync(this, entity);

        public async Task UpdateEntityAsync<U1, U2>(IEntity<U1, U2> entity, IDictionary<string, object> update_values) 
            where U2 : struct => 
            await Extensions.UpdateEntityAsync(this, entity, update_values);
    }
}
