using FCG.Domain.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FCG.Application.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        [StringLength(200, ErrorMessage = DataAnnotationsMessages.MAXLENGHT)]
        public string Nome { get; set; }

        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        [StringLength(250, ErrorMessage = DataAnnotationsMessages.MAXLENGHT)]
        public string Email { get; set; }

        [Required(ErrorMessage = DataAnnotationsMessages.REQUIRED)]
        [StringLength(50, ErrorMessage = DataAnnotationsMessages.STRINGLENGHT, MinimumLength = 8)]
        [NotMapped]
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
