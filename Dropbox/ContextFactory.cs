using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace Dropbox
{
    public class ContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder().UseSqlServer(config.GetConnectionString("sqlConnection"), b => b.MigrationsAssembly("Dropbox"));

            return new RepositoryContext(builder.Options);
        }
    }
}
