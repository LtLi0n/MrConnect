using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ServerDiscord.Boot;
using ServerDiscord.Services;
using System.IO;

namespace ServerDiscord.Design
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(AppConfig.PATH_CONFIG)
                .Build();

            var builder = new DbContextOptionsBuilder<DataContext>();
            DataContext.SyncMysqlOptions(builder, configuration);
            return new DataContext(builder.Options);
        }
    }
}
