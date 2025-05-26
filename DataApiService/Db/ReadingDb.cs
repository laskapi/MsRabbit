
using DataApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace DataApiService.Db
{
    public class ReadingDb(DbContextOptions<ReadingDb> options) : DbContext(options)
    {
        //  public ReadingDbContext(DbContextOptions<ReadingDbContext>options ) : base(options) { }
        //}

        public DbSet<Reading> Readings => Set<Reading>();
    }
}