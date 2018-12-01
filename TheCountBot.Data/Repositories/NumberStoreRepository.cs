using System.Threading.Tasks;
using TheCountBot.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace TheCountBot.Data.Repositories
{
    public class NumberStoreRepository : INumberStoreRepository
    {
        private readonly NumberStoreContext _numberStoreContext;

        public NumberStoreRepository( NumberStoreContext numberStoreContext )
        {
            _numberStoreContext = numberStoreContext;
        }

        public async Task AddNewMessageEntryAsync( MessageEntry messageEntry )
        {
            if ( !messageEntry.Correct )
            {
                messageEntry.Number = -1;
            }

            _numberStoreContext.Add( messageEntry );
            await _numberStoreContext.SaveChangesAsync();
        }

        public Task<List<MessageEntry>> GetHistoryAsync()
        {
            return Task.FromResult( _numberStoreContext.MessageEntries.OrderBy( m => m.Timestamp ).ToList() );         
        }
    }
}
