using System.ComponentModel.DataAnnotations;

namespace NexusOps.Web.Models
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Rol { get; set; } = string.Empty;
        public int IdPersonal { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
    }

    public class UsuarioGuardarDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un rol.")]
        public int IdRol { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el personal asociado.")]
        public int IdPersonal { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "El usuario debe tener entre 4 y 50 caracteres.")]
        public string NombreUsuario { get; set; } = string.Empty;

        // Obligatoria al crear; opcional al editar (si se deja vacía se conserva la actual).
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string? Password { get; set; }

        public bool Activo { get; set; } = true;
    }

    // Usuario resumido para listas desplegables (asignación de incidencias y tareas).
    public class UsuarioSimpleDto
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    public class RolDto
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public int TotalUsuarios { get; set; }
    }
}
