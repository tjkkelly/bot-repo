using TheCountBot.Data;
using System;
using System.Threading.Tasks;
using TheCountBot.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace TheCountBot.Data.Repositories
{
    public class NumberStoreRepository : INumberStoreRepository
    {
        private NumberStoreContext GetNumberStoreContext( IServiceProvider serviceProvider )
        {
            return ( NumberStoreContext ) serviceProvider.GetService( typeof( NumberStoreContext ) );
        }

        public async Task AddNewMessageEntryAsync( IServiceProvider serviceProvider, MessageEntry messageEntry )
        {
            NumberStoreContext context = GetNumberStoreContext( serviceProvider );

            context.MessageEntries.Add( messageEntry );
            await context.SaveChangesAsync();
        }

        public Task<List<MessageEntry>> GetHistoryAsync( IServiceProvider serviceProvider )
        {
            NumberStoreContext context = GetNumberStoreContext( serviceProvider );
            
            return Task.FromResult( context.MessageEntries.OrderBy( m => m.Timestamp ).ToList() );         
        }
    }
}
