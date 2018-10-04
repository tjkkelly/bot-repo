using MySql.Data.MySqlClient;  
using System;  
using System.Collections.Generic;  
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheCountBot.Data.Models;
using Microsoft.EntityFrameworkCore.Design;

namespace TheCountBot.Data
{
    public class NumberStoreContext : DbContext
    {
        public DbSet<MessageEntry> MessageEntries { get; set; }

        public NumberStoreContext( DbContextOptions<NumberStoreContext> numberStoreContextOptions )
           : base( numberStoreContextOptions )
        {
        }
    }
}
