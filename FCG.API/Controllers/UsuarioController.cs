using FCG.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using FCG.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace FCG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : BaseController
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuarioService usuarioService, ILogger<UsuarioController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpPost("Incluir")]
        public async Task<IActionResult> Incluir(UsuarioDTO usuarioDTO)
        {
            _logger.LogInformation("Iniciando inclusão de usuário. Email: {Email}, Nome: {Nome}", usuarioDTO.Email, usuarioDTO.Nome);
            try
            {
                var result = await _usuarioService.Incluir(usuarioDTO);
                _logger.LogInformation("Usuário incluído com sucesso. Email: {Email}, Id: {Id}", usuarioDTO.Email, result.Result?.Id);
                return CreateIActionResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao incluir usuário. Email: {Email}", usuarioDTO.Email);
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Alterar")]
        public async Task<IActionResult> Alterar(UsuarioDTO usuarioDTO)
        {
            _logger.LogInformation("Tentativa de alteração de usuário. Id: {Id}, Email: {Email}", usuarioDTO.Id, usuarioDTO.Email);
            try
            {
                var idUsuarioLogado = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
                var usuarioLogado = await _usuarioService.Selecionar(idUsuarioLogado);

                if (!usuarioLogado.Result.IsAdmin)
                {
                    _logger.LogWarning("Usuário não autorizado tentou alterar usuário. IdLogado: {IdLogado}, IdAlterar: {IdAlterar}", idUsuarioLogado, usuarioDTO.Id);
                    return Unauthorized("Acesso negado! Usuário não autorizado a realizar exclusão.");
                }

                var result = await _usuarioService.Alterar(usuarioDTO);
                _logger.LogInformation("Usuário alterado com sucesso. Id: {Id}, Email: {Email}", usuarioDTO.Id, usuarioDTO.Email);
                return CreateIActionResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar usuário. Id: {Id}, Email: {Email}", usuarioDTO.Id, usuarioDTO.Email);
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Excluir")]
        public async Task<IActionResult> Excluir(int id)
        {
            _logger.LogInformation("Tentativa de exclusão de usuário. Id: {Id}", id);
            try
            {
                var result = await _usuarioService.Excluir(id);
                _logger.LogInformation("Usuário excluído com sucesso. Id: {Id}", id);
                return CreateIActionResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário. Id: {Id}", id);
                throw;
            }
        }

        [HttpGet("SelecionarPorId")]
        public async Task<IActionResult> Selecionar(int id)
        {
            _logger.LogInformation("Consulta de usuário por Id. Id: {Id}", id);
            try
            {
                var result = await _usuarioService.Selecionar(id);
                _logger.LogInformation("Consulta de usuário por Id finalizada. Id: {Id}", id);
                return CreateIActionResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar usuário por Id. Id: {Id}", id);
                throw;
            }
        }

        [HttpGet("SelecionarPorNomeEmail")]
        public async Task<IActionResult> SelecionarPorNomeEmail([FromQuery] string email, [FromQuery] string nome)
        {
            _logger.LogInformation("Consulta de usuário por nome/email. Email: {Email}, Nome: {Nome}", email, nome);
            try
            {
                var result = await _usuarioService.SelecionarPorNomeEmail(email, nome);
                _logger.LogInformation("Consulta de usuário por nome/email finalizada. Email: {Email}, Nome: {Nome}", email, nome);
                return CreateIActionResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar usuário por nome/email. Email: {Email}, Nome: {Nome}", email, nome);
                throw;
            }
        }
    }
}
