using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.EntitiesConfiguration
{
    public class StoredEventConfiguration
    {
        private static ModelBuilder _builder;

        public StoredEventConfiguration(ModelBuilder builder)
        {
            _builder = builder;
            Config();
        }

        private static void Config()
        {
            _builder.Entity<StoredEvent>()
                .HasKey(e => e.Id);

            _builder.Entity<StoredEvent>()
                .Property(e => e.AggregateId)
                .IsRequired();

            _builder.Entity<StoredEvent>()
                .Property(e => e.EventType)
                .HasMaxLength(250)
                .IsRequired();

            _builder.Entity<StoredEvent>()
                .Property(e => e.Data)
                .IsRequired();

            _builder.Entity<StoredEvent>()
                .Property(e => e.Timestamp)
                .IsRequired();

            _builder.Entity<StoredEvent>()
                .Property(e => e.Version)
                .IsRequired();
        }
    }
}
