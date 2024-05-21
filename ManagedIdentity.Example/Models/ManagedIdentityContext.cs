using Microsoft.EntityFrameworkCore;

namespace ManagedIdentity.Example.Models;

public class ManagedIdentityContext : DbContext
{
    public ManagedIdentityContext(DbContextOptions<ManagedIdentityContext> options)
        : base(options)
    {
    }

    public DbSet<Info> Infos { get; set; }
}
