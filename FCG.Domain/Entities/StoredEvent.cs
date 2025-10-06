using System;

namespace FCG.Domain.Entities
{
    public class StoredEvent
    {
        public Guid Id { get; private set; }
        public string EventType { get; private set; }
        public int AggregateId { get; private set; }
        public string Data { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int Version { get; private set; }

        public StoredEvent(string eventType, int aggregateId, string data, int version)
        {
            Id = Guid.NewGuid();
            EventType = eventType;
            AggregateId = aggregateId;
            Data = data;
            Timestamp = DateTime.UtcNow;
            Version = version;
        }

        private StoredEvent()
        {
        }
    }
}
