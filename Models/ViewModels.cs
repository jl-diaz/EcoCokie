using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VoluntariadoAmbiental.Models
{
    // ViewModel para el formulario de inscripción del estudiante
    public class InscripcionViewModel
    {
        public int nIdProyecto { get; set; }
        public string sTituloProyecto { get; set; } = string.Empty;
        public string sCategoriaProyecto { get; set; } = string.Empty;
        public string sNombreMunicipio { get; set; } = string.Empty;
        public string sUbicacionDetallada { get; set; } = string.Empty;
        public string? sPuntoVerde { get; set; }
        public int nHorasAcreditar { get; set; }
        public int nCuposDisponibles { get; set; }

        // Datos del estudiante voluntario capturados mediante @Html.EditorFor
        public Voluntario Voluntario { get; set; } = new Voluntario();

        // CAMPO TOTALMENTE OPCIONAL: Marcar explícitamente como nullable para no forzar validación
        public string? sObservaciones { get; set; }
    }

    // ViewModel para la búsqueda avanzada de campañas
    public class FiltroCampanasViewModel
    {
        public string? OpcionesCat { get; set; }
        public int? OpcionesMun { get; set; }
        public string? sBusqueda { get; set; }

        public List<SelectListItem> ListaCategorias { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ListaMunicipios { get; set; } = new List<SelectListItem>();

        public List<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
    }

    // Estadísticas para el panel de métricas dinámicas
    public class DashboardStats
    {
        public int TotalProyectosActivos { get; set; }
        public int TotalVoluntariosRegistrados { get; set; }
        public int TotalHorasAcreditadas { get; set; }
        public int TotalPuntosVerdesActivos { get; set; }
        public int TotalAsistencias { get; set; }
    }
}
