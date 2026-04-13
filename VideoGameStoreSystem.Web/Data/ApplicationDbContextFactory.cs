using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using VideoGameStoreSystem.Web.Infrastructure;

namespace VideoGameStoreSystem.Web.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=mssql.lorxray.ru,14330;Database=magazine_videoigr;User Id=sanya;Password=sanya12345;TrustServerCertificate=True;Encrypt=True;");

        return new ApplicationDbContext(optionsBuilder.Options, new NullCurrentUserService());
    }
}
