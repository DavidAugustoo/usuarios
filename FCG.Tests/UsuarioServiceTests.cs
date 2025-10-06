using AutoMapper;
using FCG.Application.DTOs;
using FCG.Application.Services;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

public class UsuarioServiceTests
{
    [Fact]
    public async Task Incluir_DeveRetornarErro_SeUsuarioJaExiste()
    {
        //Arrange
        var email = "teste@fcg.com";
        var usuarioExistente = new Usuario(1, "Usuário Existente", new Email(email));

        var mockRepo = new Mock<IUsuarioRepository>();
        mockRepo.Setup(r => r.SelecionarPorEmail(email))
                .ReturnsAsync(usuarioExistente);

        var mockLogger = new Mock<ILogger<UsuarioService>>();
        var mockUow = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();

        var usuarioService = new UsuarioService(mockRepo.Object, mockLogger.Object, mockUow.Object, mockMapper.Object);

        var dto = new UsuarioDTO
        {
            Email = email,
            Nome = "Teste",
            Password = "Senha123!",
            IsAdmin = false
        };

        //Act
        var resultado = await usuarioService.Incluir(dto);

        //Assert
        Assert.NotNull(resultado);
        Assert.True(resultado.Notifications.Any());
        Assert.Contains("O usuário já existe no banco de dados!", resultado.Notifications);
        
        mockUow.Verify(u => u.Commit(), Times.Never);
    }
}
