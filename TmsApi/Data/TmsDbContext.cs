using Microsoft.EntityFrameworkCore;

namespace TmsApi.Data;

public class TmsDbContext : DbContext
{
    public TmsDbContext(DbContextOptions<TmsDbContext> options)
        : base(options)
    {
    }
}