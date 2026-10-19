using System;
using System.ComponentModel.DataAnnotations;

namespace VoluntariadoAmbiental.Models
{
    public class Voluntario
    {
        public int nIdVoluntario { get; set; }

        [Required(ErrorMessage = "El nombre completo del estudiante es obligatorio")]
        [Display(Name = "Nombre Completo del Estudiante")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres")]
        public string sNombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El carnet de estudiante es obligatorio")]
        [Display(Name = "Carnet Estudiantil / Código")]
        [StringLength(30, ErrorMessage = "Máximo 30 caracteres")]
        public string sCarnet { get; set; } = string.Empty;

        [Required(ErrorMessage = "La institución educativa es obligatoria")]
        [Display(Name = "Institución Educativa / Universidad")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres")]
        public string sInstitucion { get; set; } = "Universidad Don Bosco";

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        [Display(Name = "Correo Electrónico")]
        public string sEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono de contacto es obligatorio")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido")]
        [Display(Name = "Teléfono de Contacto / WhatsApp")]
        public string sTelefono { get; set; } = string.Empty;

        [Display(Name = "Carrera o Grado Académico")]
        public string? sCarreraOAnio { get; set; }

        public int? nIdUsuario { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime dFechaRegistro { get; set; } = DateTime.Now;
    }
}
