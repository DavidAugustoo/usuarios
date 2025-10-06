using System.Text.Json;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using FCG.Domain.EventSourcing;

namespace FCG.Infra.Data.EventSourcing
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        public EventPublisher(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }

        public async Task PublishAsync(Event @event)
        {
            var eventType = @event.GetType().Name;
            var aggregateId = @event.AggregateId;
            var eventData = JsonSerializer.Serialize(@event);
            const int version = 1;

            var storedEvent = new StoredEvent(eventType, aggregateId, eventData, version);
            await _eventStoreRepository.StoreEventAsync(storedEvent);
        }
    }
}
