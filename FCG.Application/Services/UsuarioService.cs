using AutoMapper;
using FCG.API.Models;
using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Domain.Entities;
using FCG.Domain.EventSourcing;
using FCG.Domain.EventSourcing.Events;
using FCG.Domain.Interfaces;
using FCG.Domain.Notifications;
using FCG.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FCG.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _eventPublisher;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger,
            IUnitOfWork unitOfWork, IMapper mapper, IEventPublisher eventPublisher)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _eventPublisher = eventPublisher;
        }

        public async Task<DomainNotificationsResult<UsuarioViewModel>> Incluir(UsuarioDTO usuarioDTO)
        {
            var resultNotifications = new DomainNotificationsResult<UsuarioViewModel>();
            
            _logger.LogInformation("Iniciando inclusão de usuário: {email}", usuarioDTO.Email);

            try
            {
                var usuarioRecuperado = await _usuarioRepository.SelecionarPorEmail(usuarioDTO.Email);

                if (usuarioRecuperado != null)
                {
                    _logger.LogWarning("Tentativa de inclusão de usuário já existente: {email}", usuarioDTO.Email);
                    resultNotifications.Notifications.Add("O usuário já existe no banco de dados!");
                    return resultNotifications;
                }

                var usuario = _mapper.Map<Usuario>(usuarioDTO);

                if (usuario.EmailUsuario.Notifications.Count > 0)
                {
                    _logger.LogWarning("Notificações de validação de e-mail ao incluir usuário: {email} | Notificações: {notificacoes}", usuarioDTO.Email, string.Join(", ", usuario.EmailUsuario.Notifications.Select(n => n.Message)));
                    resultNotifications.Notifications.AddRange(usuario.EmailUsuario.Notifications.Select(n => n.Message));
                    return resultNotifications;
                }

                if (usuarioDTO.Password != null)
                {
                    usuario.ValidarSenhaSegura(usuarioDTO.Password);

                    using var hmac = new HMACSHA512();
                    byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(usuarioDTO.Password));
                    byte[] passwordSalt = hmac.Key;
                    usuario.AlterarSenha(passwordHash, passwordSalt);
                }

                EnsureNome(usuario);

                await _usuarioRepository.Incluir(usuario);
                var commitResult = await _unitOfWork.Commit();

                if (!commitResult)
                {
                    _logger.LogError("Falha ao salvar usuário no banco de dados: {email}", usuarioDTO.Email);
                    resultNotifications.Notifications.Add("Não foi possível salvar o usuário.");
                    return resultNotifications;
                }

                await _eventPublisher.PublishAsync(new UsuarioCriadoEvent(usuario.Id, usuario.Nome,
                    usuario.EmailUsuario.EmailAddress, usuario.IsAdmin));

                resultNotifications.Result = _mapper.Map<UsuarioViewModel>(usuario);

                _logger.LogInformation("Usuário incluído com sucesso: {email}", usuarioDTO.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao incluir usuário: {email}", usuarioDTO.Email);
                resultNotifications.Notifications.Add("Erro ao incluir usuário.");
            }

            return resultNotifications;
        }

        public async Task<DomainNotificationsResult<UsuarioViewModel>> Alterar(UsuarioDTO usuarioDTO)
        {
            var resultNotifications = new DomainNotificationsResult<UsuarioViewModel>();

            _logger.LogInformation("Iniciando alteração de usuário: {id} - {email}", usuarioDTO.Id, usuarioDTO.Email);

            try
            {
                var usuario = await _usuarioRepository.Selecionar(usuarioDTO.Id);

                if (usuario == null)
                {
                    _logger.LogWarning("Usuário não encontrado para alteração: {id}", usuarioDTO.Id);
                    resultNotifications.Notifications.Add("Usuário não encontrado.");
                    return resultNotifications;
                }

                _mapper.Map(usuarioDTO, usuario);

                if (usuarioDTO.Password != null)
                {
                    usuario.ValidarSenhaSegura(usuarioDTO.Password);

                    using var hmac = new HMACSHA512();
                    byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(usuarioDTO.Password));
                    byte[] passwordSalt = hmac.Key;
                    usuario.AlterarSenha(passwordHash, passwordSalt);
                }

                EnsureNome(usuario);

                _usuarioRepository.Alterar(usuario);
                var commitResult = await _unitOfWork.Commit();

                if (!commitResult)
                {
                    _logger.LogError("Falha ao persistir alterações do usuário: {id}", usuarioDTO.Id);
                    resultNotifications.Notifications.Add("Não foi possível alterar o usuário.");
                    return resultNotifications;
                }

                await _eventPublisher.PublishAsync(new UsuarioAlteradoEvent(usuario.Id, usuario.Nome,
                    usuario.EmailUsuario.EmailAddress, usuario.IsAdmin));

                resultNotifications.Result = _mapper.Map<UsuarioViewModel>(usuario);

                _logger.LogInformation("Usuário alterado com sucesso: {id} - {email}", usuarioDTO.Id, usuarioDTO.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar usuário: {id} - {email}", usuarioDTO.Id, usuarioDTO.Email);
                resultNotifications.Notifications.Add("Erro ao alterar o usuário.");
            }

            return resultNotifications;
        }

        public async Task<DomainNotificationsResult<UsuarioViewModel>> Excluir(int id)
        {
            var resultNotifications = new DomainNotificationsResult<UsuarioViewModel>();

            _logger.LogInformation("Iniciando exclusão de usuário: {id}", id);

            try
            {
                var usuario = await _usuarioRepository.Excluir(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Usuário não encontrado para exclusão: {id}", id);
                    resultNotifications.Notifications.Add("Usuário não encontrado.");
                    return resultNotifications;
                }

                var commitResult = await _unitOfWork.Commit();

                if (!commitResult)
                {
                    _logger.LogError("Falha ao excluir usuário no banco de dados: {id}", id);
                    resultNotifications.Notifications.Add("Não foi possível excluir o usuário.");
                    return resultNotifications;
                }

                await _eventPublisher.PublishAsync(new UsuarioExcluidoEvent(id));

                resultNotifications.Result = _mapper.Map<UsuarioViewModel>(usuario);

                _logger.LogInformation("Usuário excluído com sucesso: {id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário: {id}", id);
                resultNotifications.Add("Erro ao excluir o usuário.");
            }

            return resultNotifications;
        }

        public async Task<DomainNotificationsResult<UsuarioViewModel>> Selecionar(int id)
        {
            var resultNotifications = new DomainNotificationsResult<UsuarioViewModel>();

            _logger.LogInformation("Selecionando usuário por ID: {id}", id);

            var usuario = await _usuarioRepository.Selecionar(id);

            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado ao selecionar por ID: {id}", id);
                resultNotifications.Notifications.Add($"Usuario com o ID {id} não encontrado.");
                return resultNotifications;
            }

            resultNotifications.Result = _mapper.Map<UsuarioViewModel>(usuario);

            _logger.LogInformation("Usuário selecionado com sucesso: {id}", id);

            return resultNotifications;
        }

        public async Task<UsuarioViewModel> SelecionarPorEmail(string email)
        {
            _logger.LogInformation("Selecionando usuário por e-mail: {email}", email);

            var usuarioSelecionado = await _usuarioRepository.SelecionarPorEmail(email);

            if (usuarioSelecionado == null)
            {
                _logger.LogWarning("Usuário não encontrado ao selecionar por e-mail: {email}", email);
                return null;
            }

            _logger.LogInformation("Usuário selecionado com sucesso por e-mail: {email}", email);

            return _mapper.Map<UsuarioViewModel>(usuarioSelecionado);
        }

        public async Task<DomainNotificationsResult<UsuarioViewModel>> SelecionarPorNomeEmail(string email, string nome)
        {
            var resultNotifications = new DomainNotificationsResult<UsuarioViewModel>();

            _logger.LogInformation("Selecionando usuário por nome ou e-mail: {nome} | {email}", nome, email);

            if (!string.IsNullOrWhiteSpace(nome))
            {
                var usuarioPorNome = await _usuarioRepository.SelecionarPorNome(nome);
                if (usuarioPorNome != null)
                {
                    _logger.LogInformation("Usuário encontrado por nome: {nome}", nome);
                    resultNotifications.Result = _mapper.Map<UsuarioViewModel>(usuarioPorNome);
                    return resultNotifications;
                }
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                var usuarioPorEmail = await _usuarioRepository.SelecionarPorEmail(email);
                if (usuarioPorEmail != null)
                {
                    _logger.LogInformation("Usuário encontrado por e-mail: {email}", email);
                    resultNotifications.Result = _mapper.Map<UsuarioViewModel>(usuarioPorEmail);
                    return resultNotifications;
                }
            }

            _logger.LogWarning("Usuário não encontrado com os dados fornecidos: {nome} | {email}", nome, email);
            resultNotifications.Notifications.Add("Usuário não encontrado com os dados fornecidos.");
            return resultNotifications;
        }

        private void EnsureNome(Usuario usuario)
        {
            if (!string.IsNullOrWhiteSpace(usuario.Nome))
            {
                return;
            }

            var fallback = usuario.EmailUsuario?.EmailAddress;

            if (string.IsNullOrWhiteSpace(fallback))
            {
                _logger.LogWarning("Usuario sem Nome e sem Email para fallback. Não será possível definir um Nome.");
                return;
            }

            var prop = usuario.GetType().GetProperty("Nome");
            if (prop == null || !prop.CanWrite)
            {
                _logger.LogWarning("Propriedade Nome não encontrada ou não gravável em Usuario. Não será possível definir um Nome.");
            }
            else
            {
                prop.SetValue(usuario, fallback);
            }

            _logger.LogInformation("Campo Nome ausente; aplicado fallback com email.");
        }
    }
}
