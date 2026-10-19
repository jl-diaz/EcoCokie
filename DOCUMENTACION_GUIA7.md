# Implementación de la Guía #7: Arquitectura Web con ASP.NET MVC

Este documento describe detalladamente la aplicación técnica de los conceptos aprendidos en la **Guía de Laboratorio #7 (Desarrollo de Aplicaciones Web con ASP.NET MVC)** en la plataforma **EcoVoluntariado**.

---

## 1. Arquitectura Modelo - Vista - Controlador (MVC)

La solución fue estructurada desacoplando por completo la lógica de negocio, la interfaz gráfica y el control de flujo:

```
VoluntariadoAmbiental/
│
├── Controllers/              <-- Controlador: Procesa peticiones y orquesta datos
│   ├── VoluntariadoController.cs
│   └── CuentaController.cs
│
├── Models/                   <-- Modelos: Entidades de datos y acceso ADO.NET
│   ├── Proyecto.cs
│   ├── Voluntario.cs
│   ├── Asistencia.cs
│   ├── Municipio.cs
│   ├── Usuario.cs
│   ├── ViewModels.cs
│   └── Conexion.cs
│
└── Views/                    <-- Vistas: Plantillas Razor (.cshtml) para el usuario
    ├── Shared/_Layout.cshtml
    ├── Voluntariado/
    │   ├── Index.cshtml
    │   ├── Detalle.cshtml
    │   ├── Inscribir.cshtml
    │   ├── PuntosVerdes.cshtml
    │   ├── Gestion.cshtml
    │   ├── Crear.cshtml
    │   ├── Editar.cshtml
    │   ├── Asistencias.cshtml
    │   └── MisHoras.cshtml
    └── Cuenta/
        ├── Login.cshtml
        ├── Registro.cshtml
        └── Perfil.cshtml
```

---

## 2. Controladores y Manejo de Peticiones (`VoluntariadoController`)

Siguiendo el modelo de la Guía #7 (donde se abordaron `HomeController`, `MenuController` y controladores por acción):

1. **Separación de Verbos HTTP (`[HttpGet]` y `[HttpPost]`):**
   * Peticiones de solo lectura (catálogo, formularios en blanco, detalle) se decoran con `[HttpGet]`.
   * Peticiones transaccionales (envío de formulario de inscripción, filtrado avanzado, edición y actualización de horas) se decoran con `[HttpPost]`.
2. **Resultados de Acción (`IActionResult` / `ActionResult`):**
   * Retorno de vistas con modelo fuertemente tipado: `return View(proyectos);`.
   * Redirecciones seguras tras procesar datos (patrón PRG: Post-Redirect-Get): `return RedirectToAction("ConfirmacionInscripcion", ...);` para evitar envíos duplicados al recargar el navegador.
3. **Manejo de Colecciones Dinámicas con `ViewBag` y `TempData`:**
   * En la Guía #7 se utilizó `ViewBag.sNumeroTarjeta` y `ViewBag.sSaldo` para transferir parámetros entre controlador y vista.
   * En el sistema, `ViewBag` se emplea para:
     * `ViewBag.OpcionesCat`: Lista de opciones para el DropDownList de categorías.
     * `ViewBag.OpcionesMun`: Lista de opciones para el DropDownList de municipios.
     * `ViewBag.Stats`: Objeto con métricas ecológicas en vivo.
   * `TempData["Exito"]` y `TempData["Error"]` transportan notificaciones flash que se muestran al usuario tras una redirección.

---

## 3. Vistas Razor y Helpers Fuertemente Tipados

### A. Formularios con `@Html.EditorFor`
Un requisito central de la práctica fue la captura de datos del estudiante mediante `@Html.EditorFor`. En [`Views/Voluntariado/Inscribir.cshtml`](file:///c:/Users/USUARIO/source/repos/VoluntariadoAmbiental/Views/Voluntariado/Inscribir.cshtml):

```csharp
@Html.LabelFor(m => m.Voluntario.sNombreCompleto, new { @class = "eco-form-label" })
@Html.EditorFor(m => m.Voluntario.sNombreCompleto, new { htmlAttributes = new { @class = "form-control eco-input", required = "required" } })
@Html.ValidationMessageFor(m => m.Voluntario.sNombreCompleto, "", new { @class = "text-danger small" })
```

* **Ventaja:** Genera los controles HTML (`<input>`) respetando los tipos de datos, validaciones y atributos de los modelos (`StringLength`, `EmailAddress`, `Phone`, etc.).
* **Campos Opcionales:** Propiedades como `sObservaciones` y `sCarreraOAnio` se configuraron como tipos nullables (`string?`) en el ViewModel, evitando validaciones implícitas indeseadas.

### B. Listas Desplegables con `@Html.DropDownList`
Al igual que en el ejercicio del cajero donde se cargaba la lista de bancos (`misbancos`), el sistema renderiza selectores dinámicos:

```csharp
@Html.DropDownList("OpcionesCat", (IEnumerable<SelectListItem>)ViewBag.OpcionesCat, new { @class = "eco-select" })
@Html.DropDownList("OpcionesMun", (IEnumerable<SelectListItem>)ViewBag.OpcionesMun, new { @class = "eco-select" })
```

---

## 4. Layout Maestro Responsivo (`_Layout.cshtml`)

Tal como se enseñó en la Guía #7 (reemplazando y estilizando `_Layout.cshtml` con Bootstrap):
* **Estructura Común:** Cabecera institucional, barra de navegación interactiva, contenedor de notificaciones y pie de página con información de la universidad.
* **Marcador `@RenderBody()`:** Renderiza el contenido específico de cada vista secundaria dentro del cuerpo del layout.
* **Responsividad Móvil:** Utiliza el sistema de grillas de Bootstrap (`col-lg-*`, `col-md-*`, `col-12`) y un menú lateral desplegable (*Offcanvas*) para garantizar navegación en teléfonos celulares y computadoras de escritorio.
