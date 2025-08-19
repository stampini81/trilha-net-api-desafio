using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TrilhaApiDesafio.Context
{
    public class DesignTimeOrganizadorContextFactory : IDesignTimeDbContextFactory<OrganizadorContext>
    {
        public OrganizadorContext CreateDbContext(string[] args)
        {
            // Build configuration to read connection string from appsettings.Development.json or appsettings.json
            var environmentName = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var basePath = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("ConexaoPadrao");

            var optionsBuilder = new DbContextOptionsBuilder<OrganizadorContext>();
            if (!string.IsNullOrWhiteSpace(connectionString) && connectionString.Contains("Data Source=", System.StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseSqlite(connectionString);
            }
            else if (!string.IsNullOrWhiteSpace(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                optionsBuilder.UseSqlite("Data Source=organizador.db");
            }

            return new OrganizadorContext(optionsBuilder.Options);
        }
    }
}
