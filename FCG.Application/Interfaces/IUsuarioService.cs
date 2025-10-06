using FCG.API.Models;
using FCG.Application.DTOs;
using FCG.Domain.Notifications;

namespace FCG.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<DomainNotificationsResult<UsuarioViewModel>> Incluir(UsuarioDTO usuarioDTO);
        Task<DomainNotificationsResult<UsuarioViewModel>> Alterar(UsuarioDTO usuarioDTO);
        Task<DomainNotificationsResult<UsuarioViewModel>> Excluir(int id);
        Task<DomainNotificationsResult<UsuarioViewModel>> Selecionar(int id);
        Task<UsuarioViewModel> SelecionarPorEmail(string email);
        Task<DomainNotificationsResult<UsuarioViewModel>> SelecionarPorNomeEmail(string email, string nome);
    }
}
