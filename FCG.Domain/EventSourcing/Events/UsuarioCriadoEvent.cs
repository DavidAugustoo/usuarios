using FCG.Domain.EventSourcing;

namespace FCG.Domain.EventSourcing.Events
{
    public class UsuarioCriadoEvent : Event
    {
        public string Nome { get; }
        public string Email { get; }
        public bool IsAdmin { get; }

        public UsuarioCriadoEvent(int usuarioId, string nome, string email, bool isAdmin)
            : base(usuarioId)
        {
            Nome = nome;
            Email = email;
            IsAdmin = isAdmin;
        }
    }
}
