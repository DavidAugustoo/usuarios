using FCG.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace FCG.API.Models
{
    public class LoginDTO
    {
        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        [DataType(DataType.EmailAddress)]
        public string EmailUsuario { get; set; } = "";

        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
