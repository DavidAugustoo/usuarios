using System.Collections.Generic;
using System.Threading.Tasks;
using FCG.Domain.Entities;

namespace FCG.Domain.EventSourcing
{
    public interface IEventStoreRepository
    {
        Task StoreEventAsync(StoredEvent storedEvent);
        Task<IEnumerable<StoredEvent>> GetEventsAsync(int aggregateId);
    }
}
