using System;
using LionLibrary.Framework;
using LionLibrary.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServerDiscord.Boot;
using SharedDiscord;

namespace ServerDiscord.Services
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<GuildEmoji> GuildEmojis { get; set; }
        
        private readonly ILogService _logger;
        private readonly IConnectionStringConfig _config;

        public DataContext(DbContextOptions<DataContext> options, ILogService logger = null) : base(options)
        {
            _logger = logger;
        }

        public DataContext(IConnectionStringConfig config, ILogService logger) : base()
        {
            _config = config;
            _logger = logger;
            SyncMysqlOptions(new DbContextOptionsBuilder(), config);
        }

        public static void SyncMysqlOptions(DbContextOptionsBuilder optionsBuilder, IConnectionStringConfig config)
        {
            optionsBuilder.UseMySql(
                $"server={config.Server};" +
                $"database={config.Database};" +
                $"user={config.User};" +
                $"password={config.Password}");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

            if (!optionsBuilder.IsConfigured)
            {
                if (_config != null) SyncMysqlOptions(optionsBuilder, _config);
                else throw new ArgumentNullException("Configuration failed.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>();
            modelBuilder.Entity<GuildEmoji>();
            _logger?.LogLine(this, "Entities configured.");
        }
    }
}
