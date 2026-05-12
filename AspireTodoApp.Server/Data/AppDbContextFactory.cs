using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AspireTodoApp.Server.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=aspire_todo_dev;Username=postgres;Password=postgres");

        var httpContextAccessor = new HttpContextAccessor();

        return new AppDbContext(optionsBuilder.Options, [], httpContextAccessor);
    }
}
