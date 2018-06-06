using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Reflection;

namespace OAuthService.Core.DbContextFactories
{
    public class ConfigurationDbContextFactory : IDesignTimeDbContextFactory<ConfigurationDbContext>
    {
        public ConfigurationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .Build();
            var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var migrationAssembly = typeof(ApplicationDbContext).GetTypeInfo().Assembly.GetName().Name;
            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationAssembly));

            return new ConfigurationDbContext(builder.Options, new ConfigurationStoreOptions());
        }
    }
}
