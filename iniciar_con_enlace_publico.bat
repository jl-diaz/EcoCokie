@echo off
title Lanzador de EcoVoluntariado con Enlace Publico HTTPS
color 0A

echo =========================================================================
echo    ECOVOLUNTARIADO - LANZADOR CON ENLACE PUBLICO HTTPS
echo =========================================================================
echo.
echo [1/2] Iniciando el servidor ASP.NET Core en segundo plano...
start "Servidor EcoVoluntariado" dotnet run --urls "http://localhost:5000"

echo Esperando 5 segundos para que el servidor este listo...
timeout /t 5 /nobreak > nul

echo.
echo [2/2] Iniciando Cloudflare Tunnel...
echo Copia el enlace que termine en ".trycloudflare.com" que aparecera a continuacion:
echo.
echo =========================================================================
cloudflared.exe tunnel --url http://localhost:5000
pause
