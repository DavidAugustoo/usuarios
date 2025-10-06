namespace FCG.API.Models
{
    public class UsuarioViewModel
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public bool IsAdmin { get; private set; }
    }
}
