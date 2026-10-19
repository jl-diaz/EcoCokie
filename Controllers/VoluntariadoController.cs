using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VoluntariadoAmbiental.Models;

namespace VoluntariadoAmbiental.Controllers
{
    /// <summary>
    /// Controlador principal de la Plataforma de Voluntariado Ambiental y Puntos Verdes
    /// Desarrollado conforme a la Guía #7 (ASP.NET MVC) y Guía #8 (ADO.NET)
    /// Integra roles: Estudiante (postulaciones), Organización (campañas y acreditación) y Admin (control total)
    /// </summary>
    public class VoluntariadoController : Controller
    {
        private readonly Conexion conexion;

        public VoluntariadoController()
        {
            conexion = new Conexion();
        }

        // =========================================================================
        // 1. VISTA PRINCIPAL: CATÁLOGO DE CAUSAS Y BÚSQUEDA AVANZADA (SqlDataAdapter)
        // =========================================================================

        [HttpGet]
        public IActionResult Index(string? categoria, int? municipio, string? busqueda)
        {
            CargarFiltrosViewBag(categoria, municipio);

            List<Proyecto> proyectos = conexion.CargarProyectos(categoria, municipio, busqueda, "Activo");
            ViewBag.Stats = conexion.getDashboardStats();
            ViewBag.CategoriaSeleccionada = categoria;
            ViewBag.MunicipioSeleccionado = municipio;
            ViewBag.BusquedaActual = busqueda;

            return View(proyectos);
        }

        [HttpPost]
        public IActionResult Index(string? OpcionesCat, string? OpcionesMun, string? sBusqueda)
        {
            int? idMun = null;
            if (!string.IsNullOrEmpty(OpcionesMun) && int.TryParse(OpcionesMun, out int parsedMun))
            {
                idMun = parsedMun;
            }

            CargarFiltrosViewBag(OpcionesCat, idMun);

            List<Proyecto> proyectos = conexion.CargarProyectos(OpcionesCat, idMun, sBusqueda, "Activo");
            ViewBag.Stats = conexion.getDashboardStats();
            ViewBag.CategoriaSeleccionada = OpcionesCat;
            ViewBag.MunicipioSeleccionado = idMun;
            ViewBag.BusquedaActual = sBusqueda;

            return View(proyectos);
        }

        // =========================================================================
        // 2. DETALLE DE PROYECTO Y CAUSA ECOLÓGICA
        // =========================================================================

        [HttpGet]
        public IActionResult Detalle(int id)
        {
            Proyecto? proyecto = conexion.mostrarProyecto(id);
            if (proyecto == null)
            {
                TempData["Error"] = "El proyecto ambiental solicitado no fue encontrado.";
                return RedirectToAction("Index");
            }

            return View(proyecto);
        }

        // =========================================================================
        // 3. FORMULARIO DE INSCRIPCIÓN DE ESTUDIANTE (@Html.EditorFor)
        // =========================================================================

        [Authorize]
        [HttpGet]
        public IActionResult Inscribir(int id)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Organizacion"))
            {
                string rol = User.IsInRole("Admin") ? "Administrador" : "Organización";
                TempData["Error"] = "Las cuentas de " + rol + " cumplen funciones de gestión y supervisión; no tienen permitido postularse como estudiantes de servicio social.";
                return RedirectToAction("Detalle", new { id = id });
            }

            Proyecto? proyecto = conexion.mostrarProyecto(id);
            if (proyecto == null)
            {
                TempData["Error"] = "No se puede inscribir: Proyecto no encontrado.";
                return RedirectToAction("Index");
            }

            if (proyecto.sEstado == "Finalizado")
            {
                TempData["Error"] = "Esta convocatoria ya ha finalizado.";
                return RedirectToAction("Index");
            }

            if (proyecto.nCuposDisponibles <= 0)
            {
                TempData["Error"] = "No hay cupos disponibles para esta actividad.";
                return RedirectToAction("Detalle", new { id = id });
            }

            Voluntario voluntario = new Voluntario
            {
                sInstitucion = "Universidad Don Bosco"
            };

            // Si el estudiante ya tiene sesión iniciada, completamos sus datos
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                voluntario.sNombreCompleto = User.Identity.Name ?? "";
                voluntario.sEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";
                voluntario.sCarnet = User.FindFirstValue("Carnet") ?? "";
                string? inst = User.FindFirstValue("Institucion");
                if (!string.IsNullOrEmpty(inst)) voluntario.sInstitucion = inst;
                voluntario.sTelefono = User.FindFirstValue("Telefono") ?? "";
            }

            InscripcionViewModel modelo = new InscripcionViewModel
            {
                nIdProyecto = proyecto.nIdProyecto,
                sTituloProyecto = proyecto.sTitulo,
                sCategoriaProyecto = proyecto.sCategoria,
                sNombreMunicipio = proyecto.sNombreMunicipio,
                sUbicacionDetallada = proyecto.sUbicacionDetallada,
                sPuntoVerde = proyecto.sPuntoVerde,
                nHorasAcreditar = proyecto.nHorasAcreditar,
                nCuposDisponibles = proyecto.nCuposDisponibles,
                Voluntario = voluntario
            };

            return View(modelo);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Inscribir(InscripcionViewModel modelo)
        {
            if (User.IsInRole("Admin") || User.IsInRole("Organizacion"))
            {
                TempData["Error"] = "Solo las cuentas con rol de Estudiante pueden postularse a convocatorias de voluntariado.";
                return RedirectToAction("Detalle", new { id = modelo.nIdProyecto });
            }

            if (!ModelState.IsValid)
            {
                RecargarDatosProyecto(modelo);
                return View(modelo);
            }

            int? idUsuarioLogueado = null;
            string? claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(claimId, out int idParsed))
            {
                idUsuarioLogueado = idParsed;
            }

            string resultado = conexion.registrarInscripcionEstudiante(modelo, idUsuarioLogueado);

            if (resultado == "OK")
            {
                TempData["Exito"] = "¡Inscripción confirmada con éxito! Te esperamos para sumar horas ecológicas.";
                return RedirectToAction("ConfirmacionInscripcion", new { idProyecto = modelo.nIdProyecto, carnet = modelo.Voluntario.sCarnet });
            }
            else
            {
                ModelState.AddModelError("", resultado);
                RecargarDatosProyecto(modelo);
                return View(modelo);
            }
        }

        [HttpGet]
        public IActionResult ConfirmacionInscripcion(int idProyecto, string carnet)
        {
            Proyecto? proyecto = conexion.mostrarProyecto(idProyecto);
            ViewBag.Proyecto = proyecto;
            ViewBag.Carnet = carnet;
            return View();
        }

        // =========================================================================
        // 4. MIS HORAS / MIS POSTULACIONES (Para Estudiantes Autenticados)
        // =========================================================================

        [Authorize]
        [HttpGet]
        public IActionResult MisHoras()
        {
            string? idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idClaim, out int idUsuario))
            {
                List<Asistencia> asistencias = conexion.getAsistenciasPorUsuario(idUsuario);
                return View(asistencias);
            }
            return RedirectToAction("Login", "Cuenta");
        }

        // =========================================================================
        // 5. PUNTOS VERDES COMUNITARIOS
        // =========================================================================

        [HttpGet]
        public IActionResult PuntosVerdes()
        {
            List<Proyecto> proyectos = conexion.CargarProyectos();
            ViewBag.Stats = conexion.getDashboardStats();
            return View(proyectos);
        }

        // =========================================================================
        // 6. GESTIÓN ADMINISTRATIVA: CICLO DE VIDA (CRUD GUÍA 8)
        // Solo para Organización y Admin
        // =========================================================================

        [Authorize(Roles = "Admin,Organizacion")]
        [HttpGet]
        public IActionResult Gestion(string? filtroEstado)
        {
            int? idOrg = null;
            // Si es Organización (no Admin), mostramos sus proyectos
            if (User.IsInRole("Organizacion"))
            {
                string? claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(claimId, out int idU)) idOrg = idU;
            }

            List<Proyecto> proyectos = conexion.CargarProyectos(null, null, null, filtroEstado, idOrg);
            ViewBag.FiltroEstado = filtroEstado;
            ViewBag.Stats = conexion.getDashboardStats();
            return View(proyectos);
        }

        // --- ALTA DE NUEVA CAMPAÑA ---
        [Authorize(Roles = "Admin,Organizacion")]
        [HttpGet]
        public IActionResult Crear()
        {
            CargarListasFormulario();
            Proyecto nuevo = new Proyecto
            {
                dFechaEvento = DateTime.Today.AddDays(7),
                nHorasAcreditar = 8,
                nCuposTotales = 30,
                sEstado = "Activo",
                sOrganizador = User.IsInRole("Organizacion") ? (User.Identity?.Name ?? "Organización") : "Universidad Don Bosco - Pastoral y Proyección Social"
            };
            return View(nuevo);
        }

        [Authorize(Roles = "Admin,Organizacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Proyecto modelo)
        {
            if (ModelState.IsValid)
            {
                int? idOrg = null;
                string? claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(claimId, out int idU)) idOrg = idU;

                int resultado = conexion.nuevoProyecto(modelo, idOrg);
                if (resultado > 0)
                {
                    TempData["Exito"] = "¡Campaña '" + modelo.sTitulo + "' creada exitosamente!";
                    return RedirectToAction("Gestion");
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo registrar la campaña en la base de datos.");
                }
            }

            CargarListasFormulario();
            return View(modelo);
        }

        // --- EDICIÓN DE CAMPAÑA ---
        [Authorize(Roles = "Admin,Organizacion")]
        [HttpGet]
        public IActionResult Editar(int id)
        {
            Proyecto? proyecto = conexion.mostrarProyecto(id);
            if (proyecto == null)
            {
                TempData["Error"] = "Campaña no encontrada.";
                return RedirectToAction("Gestion");
            }

            CargarListasFormulario();
            return View(proyecto);
        }

        [Authorize(Roles = "Admin,Organizacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Proyecto modelo)
        {
            if (ModelState.IsValid)
            {
                int resultado = conexion.actualizarProyecto(modelo);
                if (resultado > 0)
                {
                    TempData["Exito"] = "¡Campaña '" + modelo.sTitulo + "' actualizada con éxito!";
                    return RedirectToAction("Gestion");
                }
                else
                {
                    ModelState.AddModelError("", "Error al actualizar los datos en SQL Server.");
                }
            }

            CargarListasFormulario();
            return View(modelo);
        }

        // --- FINALIZAR EVENTO (BAJA DE CICLO DE VIDA) ---
        [Authorize(Roles = "Admin,Organizacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Finalizar(int id)
        {
            int resultado = conexion.finalizarProyecto(id);
            if (resultado > 0)
            {
                TempData["Exito"] = "La campaña ha sido marcada como 'Finalizada' satisfactoriamente.";
            }
            else
            {
                TempData["Error"] = "No se pudo finalizar la campaña.";
            }

            return RedirectToAction("Gestion");
        }

        // --- BAJA / ELIMINACIÓN DE CAMPAÑA (Solo Administrador) ---
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Eliminar(int id)
        {
            int resultado = conexion.eliminarProyecto(id);
            if (resultado > 0)
            {
                TempData["Exito"] = "La campaña ambiental fue eliminada definitivamente del sistema.";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar la campaña.";
            }

            return RedirectToAction("Gestion");
        }

        // =========================================================================
        // 7. CONTROL DE ASISTENCIAS Y ACTUALIZACIÓN DE HORAS POR VOLUNTARIO (Guía 8)
        // =========================================================================

        [Authorize(Roles = "Admin,Organizacion")]
        [HttpGet]
        public IActionResult Asistencias(int? idProyecto)
        {
            int? idOrg = null;
            if (User.IsInRole("Organizacion"))
            {
                string? claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(claimId, out int idU)) idOrg = idU;
            }

            List<Asistencia> asistencias = conexion.getAsistencias(idProyecto, idOrg);
            ViewBag.Proyectos = conexion.CargarProyectos(null, null, null, null, idOrg);
            ViewBag.IdProyectoSeleccionado = idProyecto;
            return View(asistencias);
        }

        [Authorize(Roles = "Admin,Organizacion")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarHoras(int nIdAsistencia, int nHorasCompletadas, string sEstadoAsistencia, string? sObservaciones, int? idProyectoRedirect)
        {
            int resultado = conexion.actualizarHorasVoluntario(nIdAsistencia, nHorasCompletadas, sEstadoAsistencia, sObservaciones);
            if (resultado > 0)
            {
                TempData["Exito"] = "Horas sociales y estado de asistencia acreditados correctamente en SQL Server.";
            }
            else
            {
                TempData["Error"] = "No se pudo actualizar el registro de horas del voluntario.";
            }

            return RedirectToAction("Asistencias", new { idProyecto = idProyectoRedirect });
        }

        // =========================================================================
        // MÉTODOS DE APOYO PRIVADOS
        // =========================================================================

        private void CargarFiltrosViewBag(string? categoriaActual, int? municipioActual)
        {
            var cats = conexion.getCategorias();
            cats.Insert(0, new SelectListItem { Text = "Todas las Categorías", Value = "Todas", Selected = string.IsNullOrEmpty(categoriaActual) || categoriaActual == "Todas" });
            ViewBag.OpcionesCat = cats;

            var muns = conexion.getMunicipios();
            muns.Insert(0, new SelectListItem { Text = "Todos los Municipios", Value = "0", Selected = !municipioActual.HasValue || municipioActual == 0 });
            ViewBag.OpcionesMun = muns;
        }

        private void CargarListasFormulario()
        {
            ViewBag.Categorias = conexion.getCategorias();
            ViewBag.Municipios = conexion.getMunicipios();

            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Text = "Activo", Value = "Activo" },
                new SelectListItem { Text = "En Curso", Value = "En Curso" },
                new SelectListItem { Text = "Finalizado", Value = "Finalizado" }
            };
        }

        private void RecargarDatosProyecto(InscripcionViewModel modelo)
        {
            var p = conexion.mostrarProyecto(modelo.nIdProyecto);
            if (p != null)
            {
                modelo.sTituloProyecto = p.sTitulo;
                modelo.sCategoriaProyecto = p.sCategoria;
                modelo.sNombreMunicipio = p.sNombreMunicipio;
                modelo.sUbicacionDetallada = p.sUbicacionDetallada;
                modelo.sPuntoVerde = p.sPuntoVerde;
                modelo.nHorasAcreditar = p.nHorasAcreditar;
                modelo.nCuposDisponibles = p.nCuposDisponibles;
            }
        }
    }
}
