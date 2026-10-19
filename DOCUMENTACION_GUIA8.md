# Implementación de la Guía #8: Conectividad ADO.NET y SQL Server

Este documento detalla la aplicación de los principios de la **Guía de Laboratorio #8 (Conectando a Fuentes de Datos con ADO.NET)** en la plataforma **EcoVoluntariado**.

---

## 1. Diseño y Estructura de la Base de Datos Relacional

Se implementó una base de datos en Microsoft SQL Server denominada `VoluntariadoVerdeDB` con cotejamiento `Modern_Spanish_CI_AS` (soporte completo para caracteres latinos, tildes y diéresis). Contiene las 4 tablas solicitadas más la tabla de usuarios:

1. **`Municipios`:** Catálogo geográfico de cobertura (`nIdMunicipio`, `sNombreMunicipio`, `sDepartamento`, `sZona`).
2. **`Proyectos`:** Convocatorias ambientales y puntos verdes comunitarios (`nIdProyecto`, `sTitulo`, `sDescripcion`, `sCategoria`, `nIdMunicipio`, `sUbicacionDetallada`, `dFechaEvento`, `nHorasAcreditar`, `nCuposTotales`, `nCuposDisponibles`, `sEstado`, `sOrganizador`, `sPuntoVerde`, `sImagenUrl`, `nIdUsuarioOrganizador`).
3. **`Voluntarios`:** Directorio de estudiantes inscritos (`nIdVoluntario`, `sNombreCompleto`, `sCarnet`, `sInstitucion`, `sEmail`, `sTelefono`, `sCarreraOAnio`, `nIdUsuario`).
4. **`Asistencias`:** Registro transaccional de horas sociales ganadas (`nIdAsistencia`, `nIdProyecto`, `nIdVoluntario`, `dFechaInscripcion`, `nHorasCompletadas`, `sEstadoAsistencia`, `sObservaciones`, `dFechaValidacion`).
5. **`Usuarios`:** Seguridad y credenciales por rol (`nIdUsuario`, `sNombreCompleto`, `sEmail`, `sPasswordHash`, `sRol`, `sCarnet`, `sInstitucion`, `sTelefono`).

---

## 2. Componentes ADO.NET Utilizados (`Models/Conexion.cs`)

En concordancia con lo enseñado en la Guía #8, la clase [`Conexion.cs`](file:///c:/Users/USUARIO/source/repos/VoluntariadoAmbiental/Models/Conexion.cs) gestiona el acceso a datos mediante los objetos fundamentales del proveedor `Microsoft.Data.SqlClient`:

| Componente ADO.NET | Propósito en el Sistema |
| :--- | :--- |
| **`SqlConnection`** | Canal de comunicación físico con SQL Server mediante cadena de conexión segura (`Server=(localdb)\MSSQLLocalDB;Database=VoluntariadoVerdeDB;...`). Métodos `conectar()`, `desconectar()` y `estadoConexion()`. |
| **`SqlCommand`** | Objeto para emitir instrucciones SQL (`INSERT`, `UPDATE`, `DELETE`, `SELECT`) y procedimientos con parámetros `@param`. |
| **`SqlDataReader`** | Cursor de solo lectura y solo avance hacia adelante, utilizado en `mostrarProyecto(id)` y para poblar los controles desplegables `getMunicipios()`. |
| **`SqlDataAdapter`** | Puente bidireccional entre la base de datos y la memoria RAM, empleado para la búsqueda avanzada combinada. |
| **`DataTable`** | Almacén en memoria desconectado que recibe los registros devueltos por el `SqlDataAdapter.Fill()`. |
| **`SqlParameter`** | Uso de parámetros fuertemente tipados (`AddWithValue`) para prevenir vulnerabilidades de inyección SQL (SQL Injection). |

---

## 3. Búsqueda Avanzada con `SqlDataAdapter` y `DataTable`

Uno de los requerimientos clave de la Guía #8 fue implementar una búsqueda combinando parámetros opcionales procesados dinámicamente mediante `SqlDataAdapter`:

```csharp
public List<Proyecto> CargarProyectos(string? categoria = null, int? idMunicipio = null, string? busqueda = null, string? estado = null)
{
    List<Proyecto> lista = new List<Proyecto>();
    if (!conectar()) return lista;

    try
    {
        string sql = @"SELECT p.*, m.sNombreMunicipio, m.sDepartamento
                       FROM Proyectos p
                       INNER JOIN Municipios m ON p.nIdMunicipio = m.nIdMunicipio
                       WHERE 1=1 ";

        SqlCommand comando = new SqlCommand();
        comando.Connection = this.conexionSQL;

        // Filtro opcional por categoría
        if (!string.IsNullOrWhiteSpace(categoria) && categoria != "0" && categoria != "Todas")
        {
            sql += " AND p.sCategoria = @sCategoria ";
            comando.Parameters.AddWithValue("@sCategoria", categoria);
        }

        // Filtro opcional por municipio
        if (idMunicipio.HasValue && idMunicipio.Value > 0)
        {
            sql += " AND p.nIdMunicipio = @nIdMunicipio ";
            comando.Parameters.AddWithValue("@nIdMunicipio", idMunicipio.Value);
        }

        // Filtro opcional por texto
        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            sql += " AND (p.sTitulo LIKE @busqueda OR p.sUbicacionDetallada LIKE @busqueda) ";
            comando.Parameters.AddWithValue("@busqueda", "%" + busqueda.Trim() + "%");
        }

        comando.CommandText = sql;

        // Ejecución mediante SqlDataAdapter y llenado de DataTable
        DataTable tablaProyectos = new DataTable();
        SqlDataAdapter dataAdapter = new SqlDataAdapter(comando);
        dataAdapter.Fill(tablaProyectos);

        // Mapeo desconectado de filas a objetos del Modelo
        foreach (DataRow fila in tablaProyectos.Rows)
        {
            lista.Add(new Proyecto { ... });
        }
    }
    finally
    {
        desconectar();
    }
    return lista;
}
```

---

## 4. Control del Ciclo de Vida Completo

La clase `Conexion` expone las operaciones fundamentales de mantenimiento de datos:

1. **Alta:** Método `nuevoProyecto(Proyecto p, int? idOrganizador)` ejecuta un `SqlCommand.ExecuteNonQuery()` insertando título, fechas, cupos iniciales, horas a acreditar y ubicación del Punto Verde.
2. **Consulta Detallada:** Método `mostrarProyecto(int id)` utiliza `SqlDataReader` para recuperar un único registro y desplegarlo en la ficha informativa o cargarlo en el formulario de edición.
3. **Modificación:** Método `actualizarProyecto(Proyecto p)` ejecuta una sentencia `UPDATE` parametrizada modificando las propiedades de la campaña.
4. **Baja y Finalización de Eventos:**
   * **Baja lógica:** Método `finalizarProyecto(int id)` actualiza el estado a `'Finalizado'`, congelando nuevas postulaciones y archivando el evento.
   * **Baja física:** Método `eliminarProyecto(int id)` ejecuta `DELETE FROM Proyectos`, aplicando eliminación en cascada si no hay dependencias.
5. **Actualización de Horas Completadas por Voluntario:**
   * Método `actualizarHorasVoluntario(idAsistencia, horas, estado, observaciones)` actualiza el registro en la tabla `Asistencias`, asigna las horas completadas y estampa automáticamente la fecha del dictamen (`dFechaValidacion = GETDATE()`).
