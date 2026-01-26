using Microsoft.EntityFrameworkCore;
using OperationalService.Storage.Entities;

namespace OperationalService.Storage;

public class RepositoryContext(DbContextOptions options) : DbContext(options: options)
{
    public DbSet<Trade> Trades { get; set; }
}
