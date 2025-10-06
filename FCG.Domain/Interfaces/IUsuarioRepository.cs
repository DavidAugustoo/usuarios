using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario> Incluir(Usuario usuario);
        void Alterar(Usuario usuario);
        Task<Usuario> Selecionar(int id);
        Task<Usuario> Excluir(int id);
        Task<Usuario> SelecionarPorEmail(string email);
        Task<Usuario> SelecionarPorNome(string nome);
    }
}
