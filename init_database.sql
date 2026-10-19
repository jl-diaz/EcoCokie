CREATE DATABASE VoluntariadoVerdeDB;
GO

USE VoluntariadoVerdeDB;
GO

-- Tabla de Usuarios
CREATE TABLE Usuarios (
    nIdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    sNombreCompleto VARCHAR(150) NOT NULL,
    sEmail VARCHAR(120) NOT NULL,
    sPasswordHash VARCHAR(250) NOT NULL,
    sRol VARCHAR(50) NOT NULL,
    sCarnet VARCHAR(30) NULL,
    sInstitucion VARCHAR(150) NULL,
    sTelefono VARCHAR(30) NULL,
    dFechaRegistro DATETIME NOT NULL
);
GO

-- Tabla de Municipios
CREATE TABLE Municipios (
    nIdMunicipio INT IDENTITY(1,1) PRIMARY KEY,
    sNombreMunicipio VARCHAR(100) NOT NULL,
    sDepartamento VARCHAR(100) NOT NULL,
    sZona VARCHAR(50) NOT NULL
);
GO

-- Tabla de Proyectos
CREATE TABLE Proyectos (
    nIdProyecto INT IDENTITY(1,1) PRIMARY KEY,
    sTitulo VARCHAR(150) NOT NULL,
    sDescripcion VARCHAR(600) NOT NULL,
    sCategoria VARCHAR(80) NOT NULL,
    nIdMunicipio INT NOT NULL,
    sUbicacionDetallada VARCHAR(250) NOT NULL,
    dFechaEvento DATETIME NOT NULL,
    nHorasAcreditar INT NOT NULL,
    nCuposTotales INT NOT NULL,
    nCuposDisponibles INT NOT NULL,
    sEstado VARCHAR(30) NOT NULL,
    sOrganizador VARCHAR(120) NOT NULL,
    sPuntoVerde VARCHAR(150) NULL,
    sImagenUrl VARCHAR(300) NULL,
    nIdUsuarioOrganizador INT NULL,
    dFechaCreacion DATETIME NOT NULL,
    FOREIGN KEY (nIdMunicipio) REFERENCES Municipios(nIdMunicipio),
    FOREIGN KEY (nIdUsuarioOrganizador) REFERENCES Usuarios(nIdUsuario)
);
GO

-- Tabla de Voluntarios
CREATE TABLE Voluntarios (
    nIdVoluntario INT IDENTITY(1,1) PRIMARY KEY,
    sNombreCompleto VARCHAR(150) NOT NULL,
    sCarnet VARCHAR(30) NOT NULL,
    sInstitucion VARCHAR(150) NOT NULL,
    sEmail VARCHAR(120) NOT NULL,
    sTelefono VARCHAR(30) NOT NULL,
    sCarreraOAnio VARCHAR(120) NULL,
    nIdUsuario INT NULL,
    dFechaRegistro DATETIME NOT NULL,
    FOREIGN KEY (nIdUsuario) REFERENCES Usuarios(nIdUsuario)
);
GO

-- Tabla de Asistencias
CREATE TABLE Asistencias (
    nIdAsistencia INT IDENTITY(1,1) PRIMARY KEY,
    nIdProyecto INT NOT NULL,
    nIdVoluntario INT NOT NULL,
    dFechaInscripcion DATETIME NOT NULL,
    nHorasCompletadas INT NOT NULL,
    sEstadoAsistencia VARCHAR(50) NOT NULL,
    sObservaciones VARCHAR(300) NULL,
    dFechaValidacion DATETIME NULL,
    FOREIGN KEY (nIdProyecto) REFERENCES Proyectos(nIdProyecto),
    FOREIGN KEY (nIdVoluntario) REFERENCES Voluntarios(nIdVoluntario)
);
GO

-- Datos iniciales de Usuarios (Contrasena: 123456)
INSERT INTO Usuarios (sNombreCompleto, sEmail, sPasswordHash, sRol, sCarnet, sInstitucion, sTelefono, dFechaRegistro) VALUES
('Administrador General EcoCokie', 'admin@ecovoluntarios.org', '123456', 'Admin', NULL, 'Universidad Don Bosco', '2251-8200', '2026-03-01'),
('Comité Ecológico UDB Verde', 'eco.udb@udb.edu.sv', '123456', 'Organizacion', NULL, 'Universidad Don Bosco - Pastoral y Proyección Social', '2251-8240', '2026-03-01'),
('Fundación Limpiemos El Salvador', 'contacto@limpiemoselsalvador.org', '123456', 'Organizacion', NULL, 'ONG Ambiental Nacional', '2222-3344', '2026-03-01'),
('Carlos Eduardo Martínez', 'carlos.martinez@udb.edu.sv', '123456', 'Estudiante', 'CM2021001', 'Universidad Don Bosco', '7123-4567', '2026-03-05'),
('Andrea Sofía Rodríguez', 'andrea.rodriguez@udb.edu.sv', '123456', 'Estudiante', 'AR2022045', 'Universidad Don Bosco', '7234-5678', '2026-03-05');
GO

-- Datos iniciales de Municipios
INSERT INTO Municipios (sNombreMunicipio, sDepartamento, sZona) VALUES
('Soyapango', 'San Salvador', 'Central'),
('San Salvador', 'San Salvador', 'Central'),
('Santa Tecla', 'La Libertad', 'Central'),
('Antiguo Cuscatlán', 'La Libertad', 'Central'),
('La Libertad (Puerto)', 'La Libertad', 'Central'),
('Santa Ana', 'Santa Ana', 'Occidental'),
('San Miguel', 'San Miguel', 'Oriental'),
('Sonsonate', 'Sonsonate', 'Occidental'),
('Ahuachapán', 'Ahuachapán', 'Occidental'),
('Chalatenango', 'Chalatenango', 'Central');
GO

-- Datos iniciales de Proyectos
INSERT INTO Proyectos (sTitulo, sDescripcion, sCategoria, nIdMunicipio, sUbicacionDetallada, dFechaEvento, nHorasAcreditar, nCuposTotales, nCuposDisponibles, sEstado, sOrganizador, sPuntoVerde, sImagenUrl, nIdUsuarioOrganizador, dFechaCreacion) VALUES
('Reforestación y Siembra de Especies Nativas Cordillera del Bálsamo', 'Jornada de reforestación masiva en áreas protegidas para mitigar erosión y enriquecer acuíferos comunitarios. Cada estudiante participará en la plantación de árboles nativos.', 'Reforestación', 3, 'Parque Ecológico San José, km 12 carretera al Volcán', '2026-04-20', 12, 40, 28, 'Activo', 'Comité Ecológico UDB Verde', 'Punto Verde El Cafetalón (Módulo Ambiental)', 'https://images.unsplash.com/photo-1542601906990-b4d3fb778b09?auto=format&fit=crop&w=1200&q=80', 2, '2026-03-10'),
('Limpieza y Clasificación de Plásticos en Costa Playa El Majahual', 'Campaña costera de recolección, pesaje y clasificación de plásticos. Apoya la conservación de tortugas marinas y manglares de la zona.', 'Limpieza de Playas', 5, 'Playa El Majahual, sector este frente al muelle artesanal', '2026-04-25', 8, 50, 34, 'Activo', 'Fundación Limpiemos El Salvador', 'Punto Verde Malecón Turístico (Tolva de Reciclaje)', 'https://images.unsplash.com/photo-1618477388954-7852f32655ec?auto=format&fit=crop&w=1200&q=80', 3, '2026-03-12'),
('Punto Verde Universitario: Eco-Canje y Reciclaje Electrónico (RAEE)', 'Operación de centro de recepción de aparatos eléctricos, metales y plásticos PET. Registro y pesaje de materiales entregados por la comunidad vecina.', 'Puntos Verdes y Reciclaje', 1, 'Universidad Don Bosco, Campus Soyapango, Plaza de los Eucaliptos', '2026-04-18', 6, 25, 15, 'Activo', 'Comité Ecológico UDB Verde', 'Punto Verde Central UDB - Estación Edificio 3', 'https://images.unsplash.com/photo-1532996122724-e3c354a0b15b?auto=format&fit=crop&w=1200&q=80', 2, '2026-03-14'),
('Saneamiento y Biobardas en Cuenca del Río Acelhuate', 'Colocación y mantenimiento de biobardas artesanales de contención flotante para retención de botellas plásticas antes del embalse Cerrón Grande.', 'Conservación de Cuencas', 2, 'Paso a desnivel Agua Caliente, ribera norte del río', '2026-04-28', 10, 30, 20, 'Activo', 'Fundación Limpiemos El Salvador', 'Punto Verde Parque Cuscatlán - EcoHub', 'https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=1200&q=80', 3, '2026-03-15'),
('Taller Comunitario de Huertos Urbanos y Compostaje Orgánico', 'Capacitación comunitaria para diseño de composteras caseras y producción de huertos urbanos con familias locales.', 'Educación Ambiental', 6, 'Centro Comunitario San Rafael, final 25 Calle Poniente', '2026-05-02', 6, 20, 12, 'Activo', 'Comité Ecológico UDB Verde', 'Punto Verde Vivero Municipal Santa Ana', 'https://images.unsplash.com/photo-1585320806297-9794b3e4eeae?auto=format&fit=crop&w=1200&q=80', 2, '2026-03-16'),
('Jornada de Limpieza Lagunar El Jocotal (Completada)', 'Extracción manual de lechuga de agua y desechos inorgánicos en el humedal con participación de voluntarios de oriente.', 'Conservación de Cuencas', 7, 'Laguna El Jocotal, Muelle de Embarque Comunal', '2026-03-20', 15, 80, 0, 'Finalizado', 'Fundación Limpiemos El Salvador', 'Punto Verde Caseta El Jocotal', 'https://images.unsplash.com/photo-1500382017468-9049fed747ef?auto=format&fit=crop&w=1200&q=80', 3, '2026-03-01');
GO

-- Datos iniciales de Voluntarios
INSERT INTO Voluntarios (sNombreCompleto, sCarnet, sInstitucion, sEmail, sTelefono, sCarreraOAnio, nIdUsuario, dFechaRegistro) VALUES
('Carlos Eduardo Martínez', 'CM2021001', 'Universidad Don Bosco', 'carlos.martinez@udb.edu.sv', '7123-4567', 'Ingeniería en Ciencias de la Computación', 4, '2026-03-05'),
('Andrea Sofía Rodríguez', 'AR2022045', 'Universidad Don Bosco', 'andrea.rodriguez@udb.edu.sv', '7234-5678', 'Ingeniería Industrial', 5, '2026-03-05'),
('Kevin Josué Henríquez', 'KH2020089', 'Universidad Don Bosco', 'kevin.henriquez@udb.edu.sv', '7345-6789', 'Licenciatura en Diseño Gráfico', NULL, '2026-03-08'),
('Mariana Estefany Rivas', 'MR2023112', 'Instituto Nacional San Salvador', 'mariana.rivas@inss.edu.sv', '7456-7890', '2do Año Bachillerato General', NULL, '2026-03-09');
GO

-- Datos iniciales de Asistencias
INSERT INTO Asistencias (nIdProyecto, nIdVoluntario, dFechaInscripcion, nHorasCompletadas, sEstadoAsistencia, sObservaciones, dFechaValidacion) VALUES
(1, 1, '2026-03-18', 0, 'Inscrito', 'Punto de encuentro: Cafetalón a las 07:00 AM', NULL),
(1, 2, '2026-03-19', 0, 'Inscrito', 'Asistirá en transporte universitario', NULL),
(2, 1, '2026-03-20', 8, 'Completado', 'Cumplió satisfactoriamente jornada costera', '2026-03-22'),
(3, 2, '2026-03-15', 6, 'Completado', 'Cumplió satisfactoriamente turno de clasificación RAEE', '2026-03-17'),
(6, 1, '2026-03-10', 15, 'Completado', 'Acreditación oficial firmada y sellada', '2026-03-21');
GO
