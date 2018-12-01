using System.Threading.Tasks;
using TheCountBot.Data.Models;
using System.Collections.Generic;

namespace TheCountBot.Data.Repositories
{
    public interface INumberStoreRepository
    {
        Task AddNewMessageEntryAsync( MessageEntry messageEntry );

        Task<List<MessageEntry>> GetHistoryAsync();
    }
}