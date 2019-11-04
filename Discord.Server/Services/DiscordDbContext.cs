using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using LionLibrary.Commands;
using LionLibrary.SQL;
using LionLibrary.Utils;
using Discord.Shared;
using Discord.Server;

namespace Discord.Server
{
    public class DiscordDbContext : DbContext
    {
        public DiscordDbContext(DbContextOptions<DiscordDbContext> options) : base(options) { }

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
            Fact.CreateModel(modelBuilder);
            FactSuggestion.CreateModel(modelBuilder);
            Guild.CreateModel(modelBuilder);

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
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DiscordDbContext>
{
    public DiscordDbContext CreateDbContext(string[] args)
    {
        AppConfig config = new AppConfig();
        var builder = new DbContextOptionsBuilder<DiscordDbContext>();
        DiscordDbContext.UseMySqlOptions(builder, config);
        return new DiscordDbContext(builder.Options);
    }
}
