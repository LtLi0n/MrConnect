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
            AppConfig config = new AppConfig();
            var builder = new DbContextOptionsBuilder<DataContext>();
            DataContext.SyncMysqlOptions(builder, config);
            return new DataContext(builder.Options);
        }
    }
}
