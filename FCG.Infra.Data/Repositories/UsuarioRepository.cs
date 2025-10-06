using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> Incluir(Usuario usuario)
        {
            await _context.Usuario.AddAsync(usuario);
            return usuario;
        }

        public void Alterar(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
        }

        public async Task<Usuario> Excluir(int id)
        {
            var usuario = await Selecionar(id);

            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
                return usuario;
            }
            else return null;
        }

        public async Task<Usuario> Selecionar(int id)
        {
            return await _context.Usuario
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Usuario> SelecionarPorEmail(string email)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.EmailUsuario.EmailAddress.ToLower() == email.ToLower());
        }

        public async Task<Usuario> SelecionarPorNome(string nome)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Nome.ToLower() == nome.ToLower());
        }
    }
}
