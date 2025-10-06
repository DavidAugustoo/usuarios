using FCG.Domain.EventSourcing;

namespace FCG.Domain.EventSourcing.Events
{
    public class UsuarioExcluidoEvent : Event
    {
        public UsuarioExcluidoEvent(int usuarioId)
            : base(usuarioId)
        {
        }
    }
}
