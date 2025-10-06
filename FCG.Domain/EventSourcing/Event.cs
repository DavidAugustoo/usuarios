using System;

namespace FCG.Domain.EventSourcing
{
    public abstract class Event
    {
        public int AggregateId { get; protected set; }
        public DateTime Timestamp { get; } = DateTime.UtcNow;

        protected Event(int aggregateId)
        {
            AggregateId = aggregateId;
        }
    }
}
