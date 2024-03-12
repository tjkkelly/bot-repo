using Microsoft.EntityFrameworkCore;
using TheCountBot.Data.Models;

namespace TheCountBot.Data
{
    public class NumberStoreContext : DbContext
    {
        public DbSet<MessageEntry> MessageEntries { get; set; }
        public DbSet<UserInsult> UserInsults { get; set; }

        public NumberStoreContext(DbContextOptions<NumberStoreContext> options) : base(options)
        {
        }
    }
}
