# Manual de Operación del Sistema y Control de Roles

**EcoVoluntariado** es una plataforma centralizada diseñada para solventar la problemática de estudiantes de bachillerato y primeros años de universidad que requieren cumplir horas de servicio social o participar en actividades ecológicas (reforestación, saneamiento de playas, recuperación de cuencas y operación de Puntos Verdes comunitarios).

---

## 1. Arquitectura de Seguridad y Roles (RBAC)

La plataforma cuenta con un sistema de control de acceso basado en roles (*Role-Based Access Control*) protegido mediante autenticación por cookies en el servidor (`Microsoft.AspNetCore.Authentication.Cookies`).

### Matriz de Permisos por Rol

| Funcionalidad / Módulo | Estudiante | Organización | Administrador | Visitante Público |
| :--- | :---: | :---: | :---: | :---: |
| Explorar catálogo de causas activas | ✅ | ✅ | ✅ | ✅ |
| Consultar directorio de Puntos Verdes | ✅ | ✅ | ✅ | ✅ |
| Ver ficha logística y de insumos | ✅ | ✅ | ✅ | ✅ |
| Inscribirse en una convocatoria | ✅ | ❌ | ❌ | ❌ (Requiere rol de Estudiante) |
| Consultar panel "Mis Horas Sociales" | ✅ | ❌ | ✅ | ❌ |
| Publicar nueva convocatoria ambiental | ❌ | ✅ | ✅ | ❌ |
| Modificar datos de una convocatoria | ❌ | ✅ (propias) | ✅ (todas) | ❌ |
| Marcar evento como finalizado | ❌ | ✅ (propias) | ✅ (todas) | ❌ |
| Consultar lista de estudiantes inscritos | ❌ | ✅ (sus eventos) | ✅ (todos) | ❌ |
| Acreditar horas y validar servicio social | ❌ | ✅ | ✅ | ❌ |
| Eliminación definitiva de registros | ❌ | ❌ | ✅ | ❌ |

---

## 2. Flujo de Uso por Rol

### A. Rol: Estudiante
1. **Inicio de Sesión:** Ingresa desde `/Cuenta/Login` con su correo estudiantil.
2. **Exploración:** En el catálogo principal filtra por categoría ecológica (ej. Reforestación) o por municipio (ej. Santa Tecla).
3. **Postulación:** Presiona *"Inscribirme"*. El sistema auto-completa su nombre, carnet, universidad y teléfono a partir de su perfil activo.
4. **Comprobante:** Al confirmar, se genera un comprobante con el número de horas meta y el Punto Verde de encuentro.
5. **Seguimiento:** En el menú superior accede a *"Mis Horas Sociales"* para monitorear el dictamen emitido por la organización y su acumulado de horas validadas.

### B. Rol: Organización
1. **Inicio de Sesión:** Ingresa con credenciales institucionales (ej. `eco.udb@udb.edu.sv`).
2. **Publicar Convocatoria:** Accede a *"Publicar Convocatoria"*, define la ubicación, horas a acreditar, cupos y el Punto Verde donde se concentrará la brigada.
3. **Supervisión de Inscritos:** En *"Mis Campañas"*, puede ver cuántos cupos quedan y revisar el listado de voluntarios registrados.
4. **Acreditación de Horas:** Al concluir la jornada, entra a *"Validar Horas"*, selecciona la actividad y presiona *"Acreditar"* en cada estudiante, asignando las horas completadas y cambiando el estado a `Completado`.

### C. Rol: Administrador
1. **Supervisión Global:** Acceso total a todas las convocatorias de cualquier organización.
2. **Mantenimiento Físico:** Autoridad exclusiva para la eliminación definitiva de proyectos obsoletos o erróneos.
3. **Auditoría:** Monitoreo de horas sociales emitidas y cupos asignados a nivel nacional.

---

## 3. Cuentas de Acceso Rápido para Demostración

Para facilitar la evaluación de la plataforma, la pantalla de inicio de sesión (`/Cuenta/Login`) incluye un panel de acceso directo de 1 clic con las siguientes credenciales precargadas:

| Rol | Correo Electrónico | Contraseña | Perfil Representado |
| :--- | :--- | :--- | :--- |
| **Estudiante** | `carlos.martinez@udb.edu.sv` | `123456` | Carlos Eduardo Martínez (Carnet: CM2021001) |
| **Organización** | `eco.udb@udb.edu.sv` | `123456` | Comité Ecológico UDB Verde |
| **Administrador** | `admin@ecovoluntarios.org` | `123456` | Administrador General del Sistema |

---

## 4. Guía de Base de Datos (SQL Server)

La base de datos `VoluntariadoVerdeDB` se administra con el script [`init_database.sql`](file:///c:/Users/USUARIO/source/repos/VoluntariadoAmbiental/init_database.sql).

### Pasos para Recrear o Migrar la Base de Datos:
1. Abra una terminal con privilegios en la carpeta del proyecto.
2. Ejecute el comando con soporte de caracteres UTF-8:
   ```powershell
   sqlcmd -S "(localdb)\MSSQLLocalDB" -f 65001 -i "init_database.sql"
   ```
3. Para utilizar un servidor SQL Server completo (ej. `.\SQLEXPRESS` o servidor en red), actualice la cadena en [`appsettings.json`](file:///c:/Users/USUARIO/source/repos/VoluntariadoAmbiental/appsettings.json) y en [`Conexion.cs`](file:///c:/Users/USUARIO/source/repos/VoluntariadoAmbiental/Models/Conexion.cs):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.\\SQLEXPRESS;Database=VoluntariadoVerdeDB;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

---

## 5. Guía de Publicación y Despliegue (Deploy)

### Opción 1: Servidor IIS (Windows Server)
1. Compilar y empaquetar en modo Release:
   ```powershell
   dotnet publish -c Release -o ./publish
   ```
2. Instalar el paquete **.NET Core Hosting Bundle** en el servidor de destino.
3. Crear un Sitio Web en el Administrador de IIS apuntando a la carpeta `./publish`.
4. Configurar el grupo de aplicaciones (*AppPool*) en modo *"Sin código administrado"*.

### Opción 2: Nube (Azure App Service)
1. Crear una base de datos en **Azure SQL Database** y ejecutar el script `init_database.sql`.
2. Crear un recurso **Azure App Service (.NET 8)**.
3. Publicar directamente desde Visual Studio mediante *Click-Once Publish* o integrando un flujo CI/CD de GitHub Actions.
