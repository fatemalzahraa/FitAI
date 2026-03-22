using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Eticaret.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer("Server=AYSENUR;Database=AvciFidancilikDb;Trusted_Connection=True;TrustServerCertificate=True;");
            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}