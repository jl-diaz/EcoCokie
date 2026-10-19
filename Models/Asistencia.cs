using System;
using System.ComponentModel.DataAnnotations;

namespace VoluntariadoAmbiental.Models
{
    public class Asistencia
    {
        public int nIdAsistencia { get; set; }

        [Required]
        [Display(Name = "Proyecto / Campaña")]
        public int nIdProyecto { get; set; }

        [Required]
        [Display(Name = "Voluntario")]
        public int nIdVoluntario { get; set; }

        [Display(Name = "Fecha de Inscripción")]
        public DateTime dFechaInscripcion { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Indique las horas completadas")]
        [Display(Name = "Horas Completadas")]
        [Range(0, 200, ErrorMessage = "El rango debe ser entre 0 y 200")]
        public int nHorasCompletadas { get; set; } = 0;

        [Required]
        [Display(Name = "Estado de Asistencia")]
        public string sEstadoAsistencia { get; set; } = "Inscrito"; // 'Inscrito', 'Completado', 'No Asistió'

        [Display(Name = "Observaciones (Opcional)")]
        [StringLength(300, ErrorMessage = "Máximo 300 caracteres")]
        public string? sObservaciones { get; set; }

        [Display(Name = "Fecha de Validación")]
        public DateTime? dFechaValidacion { get; set; }

        // Propiedades auxiliares para vistas y reportes (JOIN)
        public string sTituloProyecto { get; set; } = string.Empty;
        public string sCategoria { get; set; } = string.Empty;
        public int nHorasAcreditar { get; set; }
        public string sNombreVoluntario { get; set; } = string.Empty;
        public string sCarnet { get; set; } = string.Empty;
        public string sInstitucion { get; set; } = string.Empty;
        public string sEmail { get; set; } = string.Empty;
        public string sTelefono { get; set; } = string.Empty;
    }
}
