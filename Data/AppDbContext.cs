using Microsoft.EntityFrameworkCore;

namespace SimpleShopApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
    {
    }
}
