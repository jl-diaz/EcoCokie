using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VoluntariadoAmbiental.Models
{
    public class Municipio
    {
        public int nIdMunicipio { get; set; }
        public string sNombreMunicipio { get; set; } = string.Empty;
        public string sDepartamento { get; set; } = string.Empty;
        public string sZona { get; set; } = string.Empty;

        // Lista para dropdown de vistas según Guía 8
        public List<SelectListItem> Municipios { get; set; } = new List<SelectListItem>();
    }
}
