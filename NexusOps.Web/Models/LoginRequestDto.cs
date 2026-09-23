using System.ComponentModel.DataAnnotations;

namespace NexusOps.Web.Models
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;
    }
}
