using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VoluntariadoAmbiental.Models
{
    public class Proyecto
    {
        public int nIdProyecto { get; set; }

        [Required(ErrorMessage = "El título de la campaña es obligatorio")]
        [Display(Name = "Título de la Campaña")]
        [StringLength(150, ErrorMessage = "Máximo 150 caracteres")]
        public string sTitulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción de la causa es obligatoria")]
        [Display(Name = "Descripción del Proyecto")]
        public string sDescripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione la categoría ecológica")]
        [Display(Name = "Categoría Ecológica")]
        public string sCategoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione el municipio")]
        [Display(Name = "Municipio")]
        public int nIdMunicipio { get; set; }

        // Nombre de municipio para vistas de detalle y webgrid/cards
        [Display(Name = "Municipio")]
        public string sNombreMunicipio { get; set; } = string.Empty;
        public string sDepartamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indique la dirección o punto de encuentro")]
        [Display(Name = "Ubicación Detallada")]
        public string sUbicacionDetallada { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha del evento es obligatoria")]
        [Display(Name = "Fecha del Evento")]
        [DataType(DataType.Date)]
        public DateTime dFechaEvento { get; set; } = DateTime.Today.AddDays(7);

        [Required(ErrorMessage = "Indique las horas sociales a acreditar")]
        [Display(Name = "Horas a Acreditar")]
        [Range(1, 100, ErrorMessage = "Debe ser entre 1 y 100 horas")]
        public int nHorasAcreditar { get; set; } = 8;

        [Required(ErrorMessage = "Indique los cupos totales")]
        [Display(Name = "Cupos Totales")]
        [Range(1, 500, ErrorMessage = "Mínimo 1 cupo")]
        public int nCuposTotales { get; set; } = 30;

        [Display(Name = "Cupos Disponibles")]
        public int nCuposDisponibles { get; set; } = 30;

        [Display(Name = "Estado")]
        public string sEstado { get; set; } = "Activo";

        [Required(ErrorMessage = "Indique la entidad organizadora")]
        [Display(Name = "Organización Responsable")]
        public string sOrganizador { get; set; } = string.Empty;

        [Display(Name = "Punto Verde Comunitario (Opcional)")]
        public string? sPuntoVerde { get; set; }

        [Display(Name = "URL de Imagen (Opcional)")]
        public string? sImagenUrl { get; set; }

        public int? nIdUsuarioOrganizador { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime dFechaCreacion { get; set; } = DateTime.Now;

        // Propiedad de apoyo para lista de proyectos
        public List<Proyecto> ListaProyectos { get; set; } = new List<Proyecto>();
    }
}
