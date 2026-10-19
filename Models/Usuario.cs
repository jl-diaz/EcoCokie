using System;
using System.ComponentModel.DataAnnotations;

namespace VoluntariadoAmbiental.Models
{
    public class Usuario
    {
        public int nIdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string sNombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        [Display(Name = "Correo Electrónico")]
        public string sEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string sPasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Tipo de Cuenta / Rol")]
        public string sRol { get; set; } = "Estudiante"; // 'Estudiante', 'Organizacion', 'Admin'

        [Display(Name = "Carnet Estudiantil")]
        public string? sCarnet { get; set; }

        [Display(Name = "Institución u Organización")]
        public string? sInstitucion { get; set; }

        [Display(Name = "Teléfono de Contacto")]
        public string? sTelefono { get; set; }

        public DateTime dFechaRegistro { get; set; } = DateTime.Now;
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "Correo inválido")]
        [Display(Name = "Correo Electrónico")]
        public string sEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string sPassword { get; set; } = string.Empty;

        public bool Recordarme { get; set; } = true;

        public string? ReturnUrl { get; set; }
    }

    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre Completo / Nombre de la Organización")]
        public string sNombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
        [Display(Name = "Correo Electrónico")]
        public string sEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string sPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione su rol en el sistema")]
        [Display(Name = "Tipo de Cuenta")]
        public string sRol { get; set; } = "Estudiante"; // 'Estudiante', 'Organizacion'

        [Display(Name = "Carnet Estudiantil (Solo Estudiantes)")]
        public string? sCarnet { get; set; }

        [Display(Name = "Institución Educativa / Nombre de la ONG")]
        public string? sInstitucion { get; set; }

        [Display(Name = "Teléfono de Contacto")]
        public string? sTelefono { get; set; }
    }
}
