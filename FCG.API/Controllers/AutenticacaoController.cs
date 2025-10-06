using FCG.API.Models;
using FCG.Domain.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FCG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : BaseController
    {
        private readonly IAuthenticate _authenticateService;
        private readonly ILogger<AutenticacaoController> _logger;

        public AutenticacaoController(IAuthenticate authenticateService, ILogger<AutenticacaoController> logger)
        {
            _authenticateService = authenticateService;
            _logger = logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            _logger.LogInformation("Tentativa de login para o usuário: {email}", loginDTO.EmailUsuario);
            try
            {
                var result = await _authenticateService.Login(loginDTO);
                _logger.LogInformation("Login processado para o usuário: {email}", loginDTO.EmailUsuario);
                return CreateIActionResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar login para o usuário: {email}", loginDTO.EmailUsuario);
                throw;
            }
        }

        [HttpPost("RecuperarSenha")]
        public async Task<IActionResult> RecuperarSenha([FromQuery] string email)
        {
            _logger.LogInformation("Tentativa de recuperação de senha para o usuário: {email}", email);
            try
            {
                var result = await _authenticateService.RecuperarSenha(email);
                _logger.LogInformation("Recuperação de senha processada para o usuário: {email}", email);
                return CreateIActionResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar recuperação de senha para o usuário: {email}", email);
                throw;
            }
        }
    }
}
