using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace VoluntariadoAmbiental.Models
{
    public class Conexion
    {
        private String cadenaConexion { get; set; }
        private SqlConnection? conexionSQL;

        public Conexion()
        {
            cadenaConexion = @"Data source=(localdb)\MSSQLLocalDB;Initial Catalog=VoluntariadoVerdeDB;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true;";
        }

        // Función para realizar conexion a base de datos SQLServer (Guía 8)
        public bool conectar()
        {
            try
            {
                this.conexionSQL = new SqlConnection(this.cadenaConexion);
                this.conexionSQL.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Método que verifica si el estado de la conexion es abierta o cerrada (Guía 8)
        public bool estadoConexion()
        {
            if (this.conexionSQL == null) return false;
            switch (this.conexionSQL.State)
            {
                case System.Data.ConnectionState.Broken:
                    return true;
                case System.Data.ConnectionState.Open:
                    return true;
                default:
                    return false;
            }
        }

        public void desconectar()
        {
            if (this.conexionSQL != null)
            {
                this.conexionSQL.Close();
            }
        }

        // Validar inicio de sesión de usuario
        public Usuario? ValidarUsuario(string email, string password)
        {
            conectar();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = "SELECT * FROM Usuarios WHERE sEmail = @p1 AND sPasswordHash = @p2";
            comando.Connection = this.conexionSQL;

            try
            {
                comando.Parameters.AddWithValue("@p1", email.Trim());
                comando.Parameters.AddWithValue("@p2", password);

                lector = comando.ExecuteReader();
                Usuario? usuario = null;
                if (lector.HasRows)
                {
                    while (lector.Read())
                    {
                        usuario = new Usuario();
                        usuario.nIdUsuario = Convert.ToInt32(lector["nIdUsuario"]);
                        usuario.sNombreCompleto = Convert.ToString(lector["sNombreCompleto"]) ?? "";
                        usuario.sEmail = Convert.ToString(lector["sEmail"]) ?? "";
                        usuario.sRol = Convert.ToString(lector["sRol"]) ?? "Estudiante";
                        usuario.sCarnet = Convert.ToString(lector["sCarnet"]);
                        usuario.sInstitucion = Convert.ToString(lector["sInstitucion"]);
                        usuario.sTelefono = Convert.ToString(lector["sTelefono"]);
                        usuario.dFechaRegistro = Convert.ToDateTime(lector["dFechaRegistro"]);
                    }
                }
                lector.Close();
                desconectar();
                return usuario;
            }
            catch (SqlException)
            {
                desconectar();
                return null;
            }
        }

        // Registrar nuevo usuario
        public string RegistrarUsuario(RegistroViewModel modelo)
        {
            conectar();
            try
            {
                // Verificar si el correo ya existe
                SqlCommand cmdCheck = new SqlCommand();
                cmdCheck.CommandType = System.Data.CommandType.Text;
                cmdCheck.CommandText = "SELECT * FROM Usuarios WHERE sEmail = @p1";
                cmdCheck.Connection = this.conexionSQL;
                cmdCheck.Parameters.AddWithValue("@p1", modelo.sEmail.Trim());

                SqlDataReader lector = cmdCheck.ExecuteReader();
                bool existe = lector.HasRows;
                lector.Close();

                if (existe)
                {
                    desconectar();
                    return "El correo electrónico ya se encuentra registrado en el sistema.";
                }

                String queryInsert = "INSERT INTO Usuarios(sNombreCompleto, sEmail, sPasswordHash, sRol, sCarnet, sInstitucion, sTelefono, dFechaRegistro) VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";
                SqlCommand comando = new SqlCommand();
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = queryInsert;
                comando.Connection = this.conexionSQL;

                comando.Parameters.AddWithValue("@p1", modelo.sNombreCompleto);
                comando.Parameters.AddWithValue("@p2", modelo.sEmail.Trim());
                comando.Parameters.AddWithValue("@p3", modelo.sPassword);
                comando.Parameters.AddWithValue("@p4", modelo.sRol);
                comando.Parameters.AddWithValue("@p5", modelo.sCarnet ?? "");
                comando.Parameters.AddWithValue("@p6", modelo.sInstitucion ?? "");
                comando.Parameters.AddWithValue("@p7", modelo.sTelefono ?? "");
                comando.Parameters.AddWithValue("@p8", DateTime.Now);

                comando.ExecuteNonQuery();
                desconectar();
                return "OK";
            }
            catch (SqlException error)
            {
                desconectar();
                return "Error al registrar usuario: " + error.Message;
            }
        }

        public Usuario? ObtenerUsuarioPorId(int idUsuario)
        {
            conectar();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = "SELECT * FROM Usuarios WHERE nIdUsuario = " + idUsuario;
            comando.Connection = this.conexionSQL;

            try
            {
                lector = comando.ExecuteReader();
                Usuario? usuario = null;
                if (lector.HasRows)
                {
                    while (lector.Read())
                    {
                        usuario = new Usuario();
                        usuario.nIdUsuario = Convert.ToInt32(lector["nIdUsuario"]);
                        usuario.sNombreCompleto = Convert.ToString(lector["sNombreCompleto"]) ?? "";
                        usuario.sEmail = Convert.ToString(lector["sEmail"]) ?? "";
                        usuario.sRol = Convert.ToString(lector["sRol"]) ?? "Estudiante";
                        usuario.sCarnet = Convert.ToString(lector["sCarnet"]);
                        usuario.sInstitucion = Convert.ToString(lector["sInstitucion"]);
                        usuario.sTelefono = Convert.ToString(lector["sTelefono"]);
                        usuario.dFechaRegistro = Convert.ToDateTime(lector["dFechaRegistro"]);
                    }
                }
                lector.Close();
                desconectar();
                return usuario;
            }
            catch (SqlException)
            {
                desconectar();
                return null;
            }
        }

        // Datos para dropdown Municipios (Guía 8)
        public List<SelectListItem> getMunicipios()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            try
            {
                conectar();
                SqlCommand comando = new SqlCommand();
                SqlDataReader lector;
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "SELECT * FROM Municipios ORDER BY sNombreMunicipio";
                comando.Connection = this.conexionSQL;

                lector = comando.ExecuteReader();
                if (lector.HasRows)
                {
                    while (lector.Read())
                    {
                        items.Add(new SelectListItem
                        {
                            Text = lector["sNombreMunicipio"].ToString() + " (" + lector["sDepartamento"].ToString() + ")",
                            Value = lector["nIdMunicipio"].ToString()
                        });
                    }
                    lector.Close();
                }
                desconectar();
                return items;
            }
            catch (SqlException)
            {
                desconectar();
                return items;
            }
        }

        // Datos para dropdown Categorias (Guía 8)
        public List<SelectListItem> getCategorias()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Reforestación", Value = "Reforestación" });
            items.Add(new SelectListItem { Text = "Limpieza de Playas", Value = "Limpieza de Playas" });
            items.Add(new SelectListItem { Text = "Puntos Verdes y Reciclaje", Value = "Puntos Verdes y Reciclaje" });
            items.Add(new SelectListItem { Text = "Conservación de Cuencas", Value = "Conservación de Cuencas" });
            items.Add(new SelectListItem { Text = "Educación Ambiental", Value = "Educación Ambiental" });
            return items;
        }

        // Búsqueda de campañas usando parámetros opcionales y SqlDataAdapter (Guía 8)
        public List<Proyecto> CargarProyectos(string? categoria = null, int? idMunicipio = null, string? busqueda = null, string? estado = null, int? idOrganizador = null)
        {
            List<Proyecto> items = new List<Proyecto>();
            conectar();

            try
            {
                DataTable t = new DataTable();
                string SQL = "SELECT Proyectos.nIdProyecto, Proyectos.sTitulo, Proyectos.sDescripcion, Proyectos.sCategoria, " +
                             "Proyectos.nIdMunicipio, Municipios.sNombreMunicipio, Municipios.sDepartamento, " +
                             "Proyectos.sUbicacionDetallada, Proyectos.dFechaEvento, Proyectos.nHorasAcreditar, " +
                             "Proyectos.nCuposTotales, Proyectos.nCuposDisponibles, Proyectos.sEstado, " +
                             "Proyectos.sOrganizador, Proyectos.sPuntoVerde, Proyectos.sImagenUrl, " +
                             "Proyectos.nIdUsuarioOrganizador, Proyectos.dFechaCreacion " +
                             "FROM Municipios INNER JOIN Proyectos ON Municipios.nIdMunicipio = Proyectos.nIdMunicipio " +
                             "WHERE Proyectos.nIdProyecto > 0";

                SqlCommand comando = new SqlCommand();
                comando.Connection = this.conexionSQL;
                comando.Parameters.Clear();

                if (!string.IsNullOrEmpty(categoria) && categoria != "0" && categoria != "Todas")
                {
                    SQL = SQL + " AND Proyectos.sCategoria = @sCategoria";
                    comando.Parameters.AddWithValue("@sCategoria", categoria);
                }

                if (idMunicipio.HasValue && idMunicipio.Value > 0)
                {
                    SQL = SQL + " AND Proyectos.nIdMunicipio = @nIdMunicipio";
                    comando.Parameters.AddWithValue("@nIdMunicipio", idMunicipio.Value);
                }

                if (!string.IsNullOrEmpty(busqueda))
                {
                    SQL = SQL + " AND Proyectos.sTitulo LIKE @busqueda";
                    comando.Parameters.AddWithValue("@busqueda", "%" + busqueda.Trim() + "%");
                }

                if (!string.IsNullOrEmpty(estado))
                {
                    SQL = SQL + " AND Proyectos.sEstado = @sEstado";
                    comando.Parameters.AddWithValue("@sEstado", estado);
                }

                if (idOrganizador.HasValue && idOrganizador.Value > 0)
                {
                    SQL = SQL + " AND Proyectos.nIdUsuarioOrganizador = @idOrg";
                    comando.Parameters.AddWithValue("@idOrg", idOrganizador.Value);
                }

                SQL = SQL + " ORDER BY Proyectos.dFechaEvento";
                comando.CommandText = SQL;

                SqlDataAdapter dataAdaptador = new SqlDataAdapter(comando);
                dataAdaptador.Fill(t);

                foreach (DataRow fila in t.Rows)
                {
                    items.Add(new Proyecto
                    {
                        nIdProyecto = Convert.ToInt32(fila["nIdProyecto"]),
                        sTitulo = Convert.ToString(fila["sTitulo"]) ?? "",
                        sDescripcion = Convert.ToString(fila["sDescripcion"]) ?? "",
                        sCategoria = Convert.ToString(fila["sCategoria"]) ?? "",
                        nIdMunicipio = Convert.ToInt32(fila["nIdMunicipio"]),
                        sNombreMunicipio = Convert.ToString(fila["sNombreMunicipio"]) ?? "",
                        sDepartamento = Convert.ToString(fila["sDepartamento"]) ?? "",
                        sUbicacionDetallada = Convert.ToString(fila["sUbicacionDetallada"]) ?? "",
                        dFechaEvento = Convert.ToDateTime(fila["dFechaEvento"]),
                        nHorasAcreditar = Convert.ToInt32(fila["nHorasAcreditar"]),
                        nCuposTotales = Convert.ToInt32(fila["nCuposTotales"]),
                        nCuposDisponibles = Convert.ToInt32(fila["nCuposDisponibles"]),
                        sEstado = Convert.ToString(fila["sEstado"]) ?? "Activo",
                        sOrganizador = Convert.ToString(fila["sOrganizador"]) ?? "",
                        sPuntoVerde = Convert.ToString(fila["sPuntoVerde"]),
                        sImagenUrl = Convert.ToString(fila["sImagenUrl"]),
                        nIdUsuarioOrganizador = fila["nIdUsuarioOrganizador"] != DBNull.Value ? Convert.ToInt32(fila["nIdUsuarioOrganizador"]) : null,
                        dFechaCreacion = Convert.ToDateTime(fila["dFechaCreacion"])
                    });
                }

                desconectar();
                return items;
            }
            catch (SqlException)
            {
                desconectar();
                return items;
            }
        }

        // Método que inserta nuevo proyecto a la base de datos (Guía 8)
        public int nuevoProyecto(Proyecto p, int? idUsuarioOrganizador = null)
        {
            conectar();
            String queryInsert = "INSERT INTO Proyectos(sTitulo, sDescripcion, sCategoria, nIdMunicipio, sUbicacionDetallada, dFechaEvento, nHorasAcreditar, nCuposTotales, nCuposDisponibles, sEstado, sOrganizador, sPuntoVerde, sImagenUrl, nIdUsuarioOrganizador, dFechaCreacion) VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15)";
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = queryInsert;
            comando.Connection = this.conexionSQL;

            try
            {
                comando.Parameters.AddWithValue("@p1", p.sTitulo);
                comando.Parameters.AddWithValue("@p2", p.sDescripcion);
                comando.Parameters.AddWithValue("@p3", p.sCategoria);
                comando.Parameters.AddWithValue("@p4", p.nIdMunicipio);
                comando.Parameters.AddWithValue("@p5", p.sUbicacionDetallada);
                comando.Parameters.AddWithValue("@p6", Convert.ToDateTime(p.dFechaEvento));
                comando.Parameters.AddWithValue("@p7", p.nHorasAcreditar);
                comando.Parameters.AddWithValue("@p8", p.nCuposTotales);
                comando.Parameters.AddWithValue("@p9", p.nCuposTotales);
                comando.Parameters.AddWithValue("@p10", string.IsNullOrEmpty(p.sEstado) ? "Activo" : p.sEstado);
                comando.Parameters.AddWithValue("@p11", p.sOrganizador);
                comando.Parameters.AddWithValue("@p12", p.sPuntoVerde ?? "");
                comando.Parameters.AddWithValue("@p13", string.IsNullOrEmpty(p.sImagenUrl) ? "https://images.unsplash.com/photo-1542601906990-b4d3fb778b09?auto=format&fit=crop&w=1200&q=80" : p.sImagenUrl);
                comando.Parameters.AddWithValue("@p14", idUsuarioOrganizador.HasValue ? idUsuarioOrganizador.Value : (object)DBNull.Value);
                comando.Parameters.AddWithValue("@p15", DateTime.Now);

                int resultado = comando.ExecuteNonQuery();
                desconectar();
                return resultado;
            }
            catch (SqlException)
            {
                desconectar();
                return 0;
            }
        }

        // Método que muestra datos de un proyecto (Guía 8)
        public Proyecto? mostrarProyecto(int idProyecto)
        {
            conectar();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = "SELECT Proyectos.*, Municipios.sNombreMunicipio, Municipios.sDepartamento FROM Proyectos INNER JOIN Municipios ON Proyectos.nIdMunicipio = Municipios.nIdMunicipio WHERE Proyectos.nIdProyecto = " + idProyecto;
            comando.Connection = this.conexionSQL;

            try
            {
                lector = comando.ExecuteReader();
                Proyecto? item = null;
                if (lector.HasRows)
                {
                    while (lector.Read())
                    {
                        item = new Proyecto();
                        item.nIdProyecto = Convert.ToInt32(lector["nIdProyecto"]);
                        item.sTitulo = Convert.ToString(lector["sTitulo"]) ?? "";
                        item.sDescripcion = Convert.ToString(lector["sDescripcion"]) ?? "";
                        item.sCategoria = Convert.ToString(lector["sCategoria"]) ?? "";
                        item.nIdMunicipio = Convert.ToInt32(lector["nIdMunicipio"]);
                        item.sNombreMunicipio = Convert.ToString(lector["sNombreMunicipio"]) ?? "";
                        item.sDepartamento = Convert.ToString(lector["sDepartamento"]) ?? "";
                        item.sUbicacionDetallada = Convert.ToString(lector["sUbicacionDetallada"]) ?? "";
                        item.dFechaEvento = Convert.ToDateTime(lector["dFechaEvento"]);
                        item.nHorasAcreditar = Convert.ToInt32(lector["nHorasAcreditar"]);
                        item.nCuposTotales = Convert.ToInt32(lector["nCuposTotales"]);
                        item.nCuposDisponibles = Convert.ToInt32(lector["nCuposDisponibles"]);
                        item.sEstado = Convert.ToString(lector["sEstado"]) ?? "Activo";
                        item.sOrganizador = Convert.ToString(lector["sOrganizador"]) ?? "";
                        item.sPuntoVerde = Convert.ToString(lector["sPuntoVerde"]);
                        item.sImagenUrl = Convert.ToString(lector["sImagenUrl"]);
                        item.nIdUsuarioOrganizador = lector["nIdUsuarioOrganizador"] != DBNull.Value ? Convert.ToInt32(lector["nIdUsuarioOrganizador"]) : null;
                        item.dFechaCreacion = Convert.ToDateTime(lector["dFechaCreacion"]);
                    }
                }
                lector.Close();
                desconectar();
                return item;
            }
            catch (SqlException)
            {
                desconectar();
                return null;
            }
        }

        // Método que actualiza un proyecto (Guía 8)
        public int actualizarProyecto(Proyecto p)
        {
            conectar();
            String queryUpdate = "UPDATE Proyectos SET sTitulo=@p1, sDescripcion=@p2, sCategoria=@p3, nIdMunicipio=@p4, sUbicacionDetallada=@p5, dFechaEvento=@p6, nHorasAcreditar=@p7, nCuposTotales=@p8, sEstado=@p9, sOrganizador=@p10, sPuntoVerde=@p11, sImagenUrl=@p12 WHERE nIdProyecto=" + p.nIdProyecto;
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = queryUpdate;
            comando.Connection = this.conexionSQL;

            try
            {
                comando.Parameters.AddWithValue("@p1", p.sTitulo);
                comando.Parameters.AddWithValue("@p2", p.sDescripcion);
                comando.Parameters.AddWithValue("@p3", p.sCategoria);
                comando.Parameters.AddWithValue("@p4", p.nIdMunicipio);
                comando.Parameters.AddWithValue("@p5", p.sUbicacionDetallada);
                comando.Parameters.AddWithValue("@p6", Convert.ToDateTime(p.dFechaEvento));
                comando.Parameters.AddWithValue("@p7", p.nHorasAcreditar);
                comando.Parameters.AddWithValue("@p8", p.nCuposTotales);
                comando.Parameters.AddWithValue("@p9", p.sEstado);
                comando.Parameters.AddWithValue("@p10", p.sOrganizador);
                comando.Parameters.AddWithValue("@p11", p.sPuntoVerde ?? "");
                comando.Parameters.AddWithValue("@p12", p.sImagenUrl ?? "");

                int resultado = comando.ExecuteNonQuery();
                desconectar();
                return resultado;
            }
            catch (SqlException)
            {
                desconectar();
                return 0;
            }
        }

        // Método que elimina un proyecto (Guía 8)
        public int eliminarProyecto(int idProyecto)
        {
            conectar();
            try
            {
                // Primero eliminamos las asistencias asociadas al proyecto
                SqlCommand cmdAsis = new SqlCommand();
                cmdAsis.CommandType = System.Data.CommandType.Text;
                cmdAsis.CommandText = "DELETE FROM Asistencias WHERE nIdProyecto = @nIdProyecto";
                cmdAsis.Connection = this.conexionSQL;
                cmdAsis.Parameters.AddWithValue("@nIdProyecto", idProyecto);
                cmdAsis.ExecuteNonQuery();

                // Eliminamos el proyecto
                String queryDelete = "DELETE FROM Proyectos WHERE nIdProyecto = @nIdProyecto";
                SqlCommand comando = new SqlCommand();
                comando.Parameters.Clear();
                comando.Parameters.AddWithValue("@nIdProyecto", idProyecto);
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = queryDelete;
                comando.Connection = this.conexionSQL;

                int resultado = comando.ExecuteNonQuery();
                desconectar();
                return resultado;
            }
            catch (SqlException)
            {
                desconectar();
                return 0;
            }
        }

        // Método para dar de baja / finalizar un evento (Guía 8)
        public int finalizarProyecto(int idProyecto)
        {
            conectar();
            try
            {
                String queryUpdate = "UPDATE Proyectos SET sEstado = 'Finalizado' WHERE nIdProyecto = @nIdProyecto";
                SqlCommand comando = new SqlCommand();
                comando.Parameters.Clear();
                comando.Parameters.AddWithValue("@nIdProyecto", idProyecto);
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = queryUpdate;
                comando.Connection = this.conexionSQL;

                int resultado = comando.ExecuteNonQuery();
                desconectar();
                return resultado;
            }
            catch (SqlException)
            {
                desconectar();
                return 0;
            }
        }

        // Registrar inscripción de estudiante voluntario
        public string registrarInscripcionEstudiante(InscripcionViewModel modelo, int? idUsuarioLogueado = null)
        {
            conectar();
            try
            {
                // 1. Verificar cupos y estado del proyecto
                SqlCommand cmdProyecto = new SqlCommand();
                cmdProyecto.CommandType = System.Data.CommandType.Text;
                cmdProyecto.CommandText = "SELECT * FROM Proyectos WHERE nIdProyecto = " + modelo.nIdProyecto;
                cmdProyecto.Connection = this.conexionSQL;

                SqlDataReader lectorP = cmdProyecto.ExecuteReader();
                int cupos = 0;
                string estado = "";
                if (lectorP.HasRows && lectorP.Read())
                {
                    cupos = Convert.ToInt32(lectorP["nCuposDisponibles"]);
                    estado = Convert.ToString(lectorP["sEstado"]) ?? "";
                }
                lectorP.Close();

                if (estado == "Finalizado")
                {
                    desconectar();
                    return "Esta campaña ya ha sido finalizada.";
                }

                if (cupos <= 0)
                {
                    desconectar();
                    return "Ya no hay cupos disponibles para esta campaña.";
                }

                // 2. Buscar si el voluntario ya existe por carnet o correo
                int idVoluntario = 0;
                SqlCommand cmdVol = new SqlCommand();
                cmdVol.CommandType = System.Data.CommandType.Text;
                cmdVol.CommandText = "SELECT * FROM Voluntarios WHERE sCarnet = @p1 OR sEmail = @p2";
                cmdVol.Connection = this.conexionSQL;
                cmdVol.Parameters.AddWithValue("@p1", modelo.Voluntario.sCarnet.Trim());
                cmdVol.Parameters.AddWithValue("@p2", modelo.Voluntario.sEmail.Trim());

                SqlDataReader lectorV = cmdVol.ExecuteReader();
                if (lectorV.HasRows && lectorV.Read())
                {
                    idVoluntario = Convert.ToInt32(lectorV["nIdVoluntario"]);
                }
                lectorV.Close();

                // Si no existe, lo insertamos y luego consultamos su ID
                if (idVoluntario == 0)
                {
                    String queryInsVol = "INSERT INTO Voluntarios(sNombreCompleto, sCarnet, sInstitucion, sEmail, sTelefono, sCarreraOAnio, nIdUsuario, dFechaRegistro) VALUES(@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8)";
                    SqlCommand cmdIns = new SqlCommand();
                    cmdIns.CommandType = System.Data.CommandType.Text;
                    cmdIns.CommandText = queryInsVol;
                    cmdIns.Connection = this.conexionSQL;

                    cmdIns.Parameters.AddWithValue("@p1", modelo.Voluntario.sNombreCompleto);
                    cmdIns.Parameters.AddWithValue("@p2", modelo.Voluntario.sCarnet.Trim());
                    cmdIns.Parameters.AddWithValue("@p3", modelo.Voluntario.sInstitucion);
                    cmdIns.Parameters.AddWithValue("@p4", modelo.Voluntario.sEmail.Trim());
                    cmdIns.Parameters.AddWithValue("@p5", modelo.Voluntario.sTelefono);
                    cmdIns.Parameters.AddWithValue("@p6", modelo.Voluntario.sCarreraOAnio ?? "");
                    cmdIns.Parameters.AddWithValue("@p7", idUsuarioLogueado.HasValue ? idUsuarioLogueado.Value : (object)DBNull.Value);
                    cmdIns.Parameters.AddWithValue("@p8", DateTime.Now);
                    cmdIns.ExecuteNonQuery();

                    // Obtener el ID del voluntario recién insertado
                    SqlCommand cmdNuevoId = new SqlCommand("SELECT * FROM Voluntarios WHERE sCarnet = @p1", this.conexionSQL);
                    cmdNuevoId.Parameters.AddWithValue("@p1", modelo.Voluntario.sCarnet.Trim());
                    SqlDataReader lectorNuevo = cmdNuevoId.ExecuteReader();
                    if (lectorNuevo.HasRows && lectorNuevo.Read())
                    {
                        idVoluntario = Convert.ToInt32(lectorNuevo["nIdVoluntario"]);
                    }
                    lectorNuevo.Close();
                }

                // 3. Verificar que no esté inscrito dos veces en el mismo proyecto
                SqlCommand cmdCheckAsis = new SqlCommand();
                cmdCheckAsis.CommandType = System.Data.CommandType.Text;
                cmdCheckAsis.CommandText = "SELECT * FROM Asistencias WHERE nIdProyecto = @p1 AND nIdVoluntario = @p2";
                cmdCheckAsis.Connection = this.conexionSQL;
                cmdCheckAsis.Parameters.AddWithValue("@p1", modelo.nIdProyecto);
                cmdCheckAsis.Parameters.AddWithValue("@p2", idVoluntario);

                SqlDataReader lectorAsis = cmdCheckAsis.ExecuteReader();
                bool yaInscrito = lectorAsis.HasRows;
                lectorAsis.Close();

                if (yaInscrito)
                {
                    desconectar();
                    return "El estudiante ya se encuentra inscrito en esta convocatoria.";
                }

                // 4. Insertar la asistencia
                String queryInsAsis = "INSERT INTO Asistencias(nIdProyecto, nIdVoluntario, dFechaInscripcion, nHorasCompletadas, sEstadoAsistencia, sObservaciones) VALUES(@p1, @p2, @p3, @p4, @p5, @p6)";
                SqlCommand cmdAsis = new SqlCommand();
                cmdAsis.CommandType = System.Data.CommandType.Text;
                cmdAsis.CommandText = queryInsAsis;
                cmdAsis.Connection = this.conexionSQL;

                cmdAsis.Parameters.AddWithValue("@p1", modelo.nIdProyecto);
                cmdAsis.Parameters.AddWithValue("@p2", idVoluntario);
                cmdAsis.Parameters.AddWithValue("@p3", DateTime.Now);
                cmdAsis.Parameters.AddWithValue("@p4", 0);
                cmdAsis.Parameters.AddWithValue("@p5", "Inscrito");
                cmdAsis.Parameters.AddWithValue("@p6", modelo.sObservaciones ?? "");
                cmdAsis.ExecuteNonQuery();

                // 5. Restar 1 cupo disponible al proyecto
                SqlCommand cmdCupos = new SqlCommand();
                cmdCupos.CommandType = System.Data.CommandType.Text;
                cmdCupos.CommandText = "UPDATE Proyectos SET nCuposDisponibles = @p1 WHERE nIdProyecto = @p2";
                cmdCupos.Connection = this.conexionSQL;
                cmdCupos.Parameters.AddWithValue("@p1", cupos - 1);
                cmdCupos.Parameters.AddWithValue("@p2", modelo.nIdProyecto);
                cmdCupos.ExecuteNonQuery();

                desconectar();
                return "OK";
            }
            catch (SqlException error)
            {
                desconectar();
                return "Error al procesar inscripción: " + error.Message;
            }
        }

        // Consultar asistencias para organizaciones y administradores
        public List<Asistencia> getAsistencias(int? idProyecto = null, int? idOrganizador = null)
        {
            List<Asistencia> lista = new List<Asistencia>();
            conectar();

            try
            {
                string SQL = "SELECT Asistencias.nIdAsistencia, Asistencias.nIdProyecto, Asistencias.nIdVoluntario, " +
                             "Asistencias.dFechaInscripcion, Asistencias.nHorasCompletadas, Asistencias.sEstadoAsistencia, " +
                             "Asistencias.sObservaciones, Asistencias.dFechaValidacion, Proyectos.sTitulo, Proyectos.sCategoria, " +
                             "Proyectos.nHorasAcreditar, Voluntarios.sNombreCompleto, Voluntarios.sCarnet, " +
                             "Voluntarios.sInstitucion, Voluntarios.sEmail, Voluntarios.sTelefono " +
                             "FROM Asistencias INNER JOIN Proyectos ON Asistencias.nIdProyecto = Proyectos.nIdProyecto " +
                             "INNER JOIN Voluntarios ON Asistencias.nIdVoluntario = Voluntarios.nIdVoluntario " +
                             "WHERE Asistencias.nIdAsistencia > 0";

                SqlCommand comando = new SqlCommand();
                comando.Connection = this.conexionSQL;

                if (idProyecto.HasValue && idProyecto.Value > 0)
                {
                    SQL = SQL + " AND Asistencias.nIdProyecto = @idP";
                    comando.Parameters.AddWithValue("@idP", idProyecto.Value);
                }

                if (idOrganizador.HasValue && idOrganizador.Value > 0)
                {
                    SQL = SQL + " AND Proyectos.nIdUsuarioOrganizador = @idOrg";
                    comando.Parameters.AddWithValue("@idOrg", idOrganizador.Value);
                }

                comando.CommandText = SQL;
                SqlDataReader lector = comando.ExecuteReader();
                if (lector.HasRows)
                {
                    while (lector.Read())
                    {
                        lista.Add(new Asistencia
                        {
                            nIdAsistencia = Convert.ToInt32(lector["nIdAsistencia"]),
                            nIdProyecto = Convert.ToInt32(lector["nIdProyecto"]),
                            nIdVoluntario = Convert.ToInt32(lector["nIdVoluntario"]),
                            dFechaInscripcion = Convert.ToDateTime(lector["dFechaInscripcion"]),
                            nHorasCompletadas = Convert.ToInt32(lector["nHorasCompletadas"]),
                            sEstadoAsistencia = Convert.ToString(lector["sEstadoAsistencia"]) ?? "Inscrito",
                            sObservaciones = Convert.ToString(lector["sObservaciones"]),
                            dFechaValidacion = lector["dFechaValidacion"] != DBNull.Value ? Convert.ToDateTime(lector["dFechaValidacion"]) : null,
                            sTituloProyecto = Convert.ToString(lector["sTitulo"]) ?? "",
                            sCategoria = Convert.ToString(lector["sCategoria"]) ?? "",
                            nHorasAcreditar = Convert.ToInt32(lector["nHorasAcreditar"]),
                            sNombreVoluntario = Convert.ToString(lector["sNombreCompleto"]) ?? "",
                            sCarnet = Convert.ToString(lector["sCarnet"]) ?? "",
                            sInstitucion = Convert.ToString(lector["sInstitucion"]) ?? "",
                            sEmail = Convert.ToString(lector["sEmail"]) ?? "",
                            sTelefono = Convert.ToString(lector["sTelefono"]) ?? ""
                        });
                    }
                }
                lector.Close();
                desconectar();
                return lista;
            }
            catch (SqlException)
            {
                desconectar();
                return lista;
            }
        }

        // Consultar asistencias del estudiante que inició sesión
        public List<Asistencia> getAsistenciasPorUsuario(int idUsuario)
        {
            List<Asistencia> lista = new List<Asistencia>();
            conectar();

            try
            {
                string SQL = "SELECT Asistencias.nIdAsistencia, Asistencias.nIdProyecto, Asistencias.nIdVoluntario, " +
                             "Asistencias.dFechaInscripcion, Asistencias.nHorasCompletadas, Asistencias.sEstadoAsistencia, " +
                             "Asistencias.sObservaciones, Asistencias.dFechaValidacion, Proyectos.sTitulo, Proyectos.sCategoria, " +
                             "Proyectos.nHorasAcreditar, Voluntarios.sNombreCompleto, Voluntarios.sCarnet, " +
                             "Voluntarios.sInstitucion, Voluntarios.sEmail, Voluntarios.sTelefono " +
                             "FROM Asistencias INNER JOIN Proyectos ON Asistencias.nIdProyecto = Proyectos.nIdProyecto " +
                             "INNER JOIN Voluntarios ON Asistencias.nIdVoluntario = Voluntarios.nIdVoluntario " +
                             "WHERE Voluntarios.nIdUsuario = @idUser";

                SqlCommand comando = new SqlCommand(SQL, this.conexionSQL);
                comando.Parameters.AddWithValue("@idUser", idUsuario);

                SqlDataReader lector = comando.ExecuteReader();
                if (lector.HasRows)
                {
                    while (lector.Read())
                    {
                        lista.Add(new Asistencia
                        {
                            nIdAsistencia = Convert.ToInt32(lector["nIdAsistencia"]),
                            nIdProyecto = Convert.ToInt32(lector["nIdProyecto"]),
                            nIdVoluntario = Convert.ToInt32(lector["nIdVoluntario"]),
                            dFechaInscripcion = Convert.ToDateTime(lector["dFechaInscripcion"]),
                            nHorasCompletadas = Convert.ToInt32(lector["nHorasCompletadas"]),
                            sEstadoAsistencia = Convert.ToString(lector["sEstadoAsistencia"]) ?? "Inscrito",
                            sObservaciones = Convert.ToString(lector["sObservaciones"]),
                            dFechaValidacion = lector["dFechaValidacion"] != DBNull.Value ? Convert.ToDateTime(lector["dFechaValidacion"]) : null,
                            sTituloProyecto = Convert.ToString(lector["sTitulo"]) ?? "",
                            sCategoria = Convert.ToString(lector["sCategoria"]) ?? "",
                            nHorasAcreditar = Convert.ToInt32(lector["nHorasAcreditar"]),
                            sNombreVoluntario = Convert.ToString(lector["sNombreCompleto"]) ?? "",
                            sCarnet = Convert.ToString(lector["sCarnet"]) ?? "",
                            sInstitucion = Convert.ToString(lector["sInstitucion"]) ?? "",
                            sEmail = Convert.ToString(lector["sEmail"]) ?? "",
                            sTelefono = Convert.ToString(lector["sTelefono"]) ?? ""
                        });
                    }
                }
                lector.Close();
                desconectar();
                return lista;
            }
            catch (SqlException)
            {
                desconectar();
                return lista;
            }
        }

        // Método que actualiza las horas completadas por voluntario (Guía 8)
        public int actualizarHorasVoluntario(int idAsistencia, int horasCompletadas, string estadoAsistencia, string? observaciones)
        {
            conectar();
            String queryUpdate = "UPDATE Asistencias SET nHorasCompletadas=@p1, sEstadoAsistencia=@p2, sObservaciones=@p3, dFechaValidacion=@p4 WHERE nIdAsistencia=" + idAsistencia;
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = queryUpdate;
            comando.Connection = this.conexionSQL;

            try
            {
                comando.Parameters.AddWithValue("@p1", horasCompletadas);
                comando.Parameters.AddWithValue("@p2", estadoAsistencia);
                comando.Parameters.AddWithValue("@p3", observaciones ?? "");
                comando.Parameters.AddWithValue("@p4", DateTime.Now);

                int resultado = comando.ExecuteNonQuery();
                desconectar();
                return resultado;
            }
            catch (SqlException)
            {
                desconectar();
                return 0;
            }
        }

        // Contadores generales usando lecturas simples con SqlDataReader
        public DashboardStats getDashboardStats()
        {
            DashboardStats stats = new DashboardStats();
            conectar();

            try
            {
                // Contar proyectos activos y puntos verdes
                SqlCommand cmdP = new SqlCommand("SELECT * FROM Proyectos", this.conexionSQL);
                SqlDataReader lectorP = cmdP.ExecuteReader();
                if (lectorP.HasRows)
                {
                    while (lectorP.Read())
                    {
                        string est = Convert.ToString(lectorP["sEstado"]) ?? "";
                        string pv = Convert.ToString(lectorP["sPuntoVerde"]) ?? "";
                        if (est == "Activo") stats.TotalProyectosActivos++;
                        if (pv != "") stats.TotalPuntosVerdesActivos++;
                    }
                }
                lectorP.Close();

                // Contar voluntarios
                SqlCommand cmdV = new SqlCommand("SELECT * FROM Voluntarios", this.conexionSQL);
                SqlDataReader lectorV = cmdV.ExecuteReader();
                if (lectorV.HasRows)
                {
                    while (lectorV.Read())
                    {
                        stats.TotalVoluntariosRegistrados++;
                    }
                }
                lectorV.Close();

                // Sumar horas completadas y contar asistencias
                SqlCommand cmdA = new SqlCommand("SELECT * FROM Asistencias", this.conexionSQL);
                SqlDataReader lectorA = cmdA.ExecuteReader();
                if (lectorA.HasRows)
                {
                    while (lectorA.Read())
                    {
                        stats.TotalAsistencias++;
                        stats.TotalHorasAcreditadas = stats.TotalHorasAcreditadas + Convert.ToInt32(lectorA["nHorasCompletadas"]);
                    }
                }
                lectorA.Close();

                desconectar();
            }
            catch (SqlException)
            {
                desconectar();
            }

            return stats;
        }
    }
}
