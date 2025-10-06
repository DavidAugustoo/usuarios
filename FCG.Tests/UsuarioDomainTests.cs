using FCG.Domain.Entities;
using FCG.Domain.Validation;
using FCG.Domain.ValueObjects;

namespace FCG.Tests
{
    public class UsuarioDomainTests
    {
        [Fact]
        public void SenhaSeguraDevePassar()
        {
            var usuario = new Usuario();
            var senhaValida = "Senha123!";

            Exception ex = Record.Exception(() => usuario.ValidarSenhaSegura(senhaValida));

            Assert.Null(ex);
        }

        [Theory]
        [InlineData("1234567")]
        [InlineData("senhasemletramaiuscula!")]
        [InlineData("SENHASEMNÚMEROS")]
        
        public void SenhaNaoSeguraDeveFalhar(string senhaInvalida)
        {
            var usuario = new Usuario();

            Assert.Throws<DomainExceptionValidation>(() => usuario.ValidarSenhaSegura(senhaInvalida));
        }

        [Fact]
        public void UsuarioPadraoNaoPodeSerAdmin()
        {
            var email = new Email("teste@fiap.com");
            var usuario = new Usuario(1, "Vinícius", email);

            Assert.False(usuario.IsAdmin);
        }

        [Fact]
        public void UsuarioDevePermitirPromoverAdmin()
        {
            var email = new Email("admin@fiap.com");
            var usuario = new Usuario(1, "Administrador", email);

            usuario.PromoverAdmin();

            Assert.True(usuario.IsAdmin);
        }
    }
}