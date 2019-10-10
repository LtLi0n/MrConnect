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
using WoT.Server.Boot;
using Microsoft.EntityFrameworkCore.Design;
using WoT.Shared;
using LionLibrary.Utils;

namespace WoT.Server.Services
{
    public class WoTDbContext : DbContext
    {
        ///<summary>users</summary>
        public DbSet<User> Users { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterWork> CharactersWork { get; set; }

        private readonly IServiceProvider _services;

        public WoTDbContext(DbContextOptions<WoTDbContext> options) : base(options) { }
            

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

            if (!optionsBuilder.IsConfigured)
            {
                throw new ArgumentNullException("Configuration failed.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            User.CreateModel(modelBuilder);
            Character.CreateModel(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        ///<summary>Adds an object to the database.</summary>
        public async Task<U2> AddEntityAsync<U1, U2>(IEntity<U1, U2> obj, DbSet<U1> db_set = null)
            where U1 : class
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
            where U1 : class =>
            await Extensions.UpdateEntityAsync(this, entity);

        public async Task UpdateEntityAsync<U1, U2>(IEntity<U1, U2> entity, IDictionary<string, object> update_values) 
            where U1 : class => 
            await Extensions.UpdateEntityAsync(this, entity, update_values);

        public static void UseMySqlOptions(DbContextOptionsBuilder optionsBuilder, IConnectionStringConfig config) =>
            optionsBuilder.UseMySql(
                $"server={config.Server};" +
                $"database={config.Database};" +
                $"user={config.User};" +
                $"password={config.Password}");
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<WoTDbContext>
    {
        public WoTDbContext CreateDbContext(string[] args)
        {
            AppConfig config = new AppConfig();
            var builder = new DbContextOptionsBuilder<WoTDbContext>();
            WoTDbContext.UseMySqlOptions(builder, config);
            return new WoTDbContext(builder.Options);
        }
    }
}
