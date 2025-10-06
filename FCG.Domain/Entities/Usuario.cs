using FCG.Domain.Validation;
using FCG.Domain.ValueObjects;

namespace FCG.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; private set; }
        public string Nome { get; private set; }
        public Email EmailUsuario { get; private set; }
        public bool IsAdmin { get; private set; }
        public byte[] PasswordHash { get; private set; } = new byte[32];
        public byte[] PasswordSalt { get; private set; } = new byte[32];

        public Usuario() { }

        public Usuario(int id, string nome, Email email)
        {
            Id = id;
            Nome = nome;
            EmailUsuario = email;
            IsAdmin = false;
        }

        public void AlterarSenha(byte[] passwordHash, byte[] passwordSalt)
        {
            PasswordSalt = passwordSalt;
            PasswordHash = passwordHash;
        }

        public void ValidarSenhaSegura(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
                DomainExceptionValidation.When(string.IsNullOrWhiteSpace(senha) || senha.Length < 8, "A senha deve ter ao menos 8 caracteres!");

            bool temMaiuscula = senha.Any(char.IsUpper);
            bool temMinuscula = senha.Any(char.IsLower);
            bool temNumero = senha.Any(char.IsDigit);
            bool temEspecial = senha.Any(c => !char.IsLetterOrDigit(c));

            DomainExceptionValidation.When(!(temMaiuscula && temMinuscula && temNumero && temEspecial),
                "A senha deve possuir ao menos 8 caracteres que, " +
                "contenha ao menos 1 letra minúscula, 1 letra maiúscula e 1 caractere especial"
                );
        }

        public string GerarSenhaSeguraTemporaria()
        {
            const int length = 8;
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()-_=+";

            var random = new Random();
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(chars);
        }

        public void PromoverAdmin() => IsAdmin = true;

    }
}
