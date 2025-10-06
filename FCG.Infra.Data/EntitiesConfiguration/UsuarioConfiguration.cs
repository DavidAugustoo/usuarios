using FCG.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.EntitiesConfiguration
{
    public class UsuarioConfiguration
    {
        private static ModelBuilder _builder;
        public UsuarioConfiguration(ModelBuilder builder)
        {
            _builder = builder;
            Config();
        }

        private static void Config()
        {
            _builder.Entity<Usuario>()
                .HasKey(x => x.Id);

            _builder.Entity<Usuario>()
                .Property(x => x.Nome)
                .HasMaxLength(200)
                .IsRequired();

            _builder.Entity<Usuario>()
                 .OwnsOne(x => x.EmailUsuario, email =>
                 {
                     email.Property(e => e.EmailAddress)
                          .HasColumnName("Email")
                          .HasMaxLength(250)
                          .IsRequired();
                     email.Ignore(e => e.Notifications);
                 });

            _builder.Entity<Usuario>()
                .Property(x => x.IsAdmin)
                .IsRequired();
        }
    }
}
