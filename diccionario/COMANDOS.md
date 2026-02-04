# 🔥 Guía Rápida de Comandos

## Setup Inicial (Primera vez)

```bash
# 1. Levantar PostgreSQL
cd docker
docker-compose up -d
cd ..

# 2. Restaurar paquetes
dotnet restore

# 3. Crear migración inicial
cd src/BusinessApp.Web
dotnet ef migrations add InitialCreate --project ../BusinessApp.Infrastructure

# 4. Aplicar migración
dotnet ef database update --project ../BusinessApp.Infrastructure

# 5. Ejecutar aplicación
dotnet run
```

O simplemente ejecuta el script automatizado:
```bash
./setup.sh
```

## Desarrollo Diario

```bash
# Iniciar PostgreSQL (si no está corriendo)
cd docker && docker-compose up -d && cd ..

# Ejecutar la aplicación
cd src/BusinessApp.Web
dotnet run

# Ver logs en tiempo real
tail -f logs/businessapp-*.txt
```

## Migraciones

```bash
# Crear nueva migración
cd src/BusinessApp.Web
dotnet ef migrations add NombreMigracion --project ../BusinessApp.Infrastructure

# Aplicar migraciones pendientes
dotnet ef database update --project ../BusinessApp.Infrastructure

# Rollback a una migración específica
dotnet ef database update NombreMigracion --project ../BusinessApp.Infrastructure

# Eliminar última migración (NO aplicada)
dotnet ef migrations remove --project ../BusinessApp.Infrastructure

# Ver migraciones aplicadas
dotnet ef migrations list --project ../BusinessApp.Infrastructure

# Generar script SQL
dotnet ef migrations script --project ../BusinessApp.Infrastructure --output migration.sql
```

## Docker

```bash
# Iniciar servicios
cd docker
docker-compose up -d

# Ver logs
docker-compose logs -f postgres

# Detener servicios
docker-compose down

# Detener y eliminar volúmenes (⚠️ BORRA LA DB)
docker-compose down -v

# Reiniciar PostgreSQL
docker-compose restart postgres

# Conectarse a PostgreSQL via CLI
docker exec -it businessapp_postgres psql -U postgres -d businessapp
```

## Base de Datos

```bash
# Backup de la base de datos
docker exec businessapp_postgres pg_dump -U postgres businessapp > backup.sql

# Restaurar backup
docker exec -i businessapp_postgres psql -U postgres businessapp < backup.sql

# Resetear la base de datos (⚠️ BORRA TODO)
cd src/BusinessApp.Web
dotnet ef database drop --project ../BusinessApp.Infrastructure --force
dotnet ef database update --project ../BusinessApp.Infrastructure
```

## Build y Publish

```bash
# Build en modo Debug
dotnet build

# Build en modo Release
dotnet build -c Release

# Publish (para producción)
cd src/BusinessApp.Web
dotnet publish -c Release -o ../../publish

# Ejecutar versión publicada
cd ../../publish
./BusinessApp.Web
```

## Testing (cuando implementes tests)

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con coverage
dotnet test /p:CollectCoverage=true
```

## Limpieza

```bash
# Limpiar archivos de build
dotnet clean

# Limpiar + eliminar bin y obj
find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +

# Restaurar desde cero
dotnet clean
dotnet restore
dotnet build
```

## Agregar Paquetes NuGet

```bash
# Ejemplo: Agregar paquete a Infrastructure
cd src/BusinessApp.Infrastructure
dotnet add package NombreDelPaquete

# Ejemplo: Agregar versión específica
dotnet add package NombreDelPaquete --version 1.2.3
```

## Git (cuando inicies el repo)

```bash
# Inicializar repo
git init

# Primer commit
git add .
git commit -m "Initial commit - Fase 1 completada"

# Conectar a repo remoto
git remote add origin https://github.com/tu-usuario/BusinessApp.git
git push -u origin main
```

## URLs Útiles

- **Aplicación:** https://localhost:7001 o http://localhost:5001
- **PgAdmin:** http://localhost:5050
  - Email: admin@businessapp.com
  - Password: admin
- **PostgreSQL:** localhost:5432
  - Database: businessapp
  - User: postgres
  - Password: postgres

## Credenciales por Defecto

**Usuario Admin:**
- Email: admin@businessapp.com
- Password: Admin123!

**PgAdmin:**
- Email: admin@businessapp.com
- Password: admin

## Troubleshooting

```bash
# Si hay problemas con puertos
netstat -an | grep 5432  # PostgreSQL
netstat -an | grep 5001  # App

# Si PostgreSQL no inicia
docker-compose down
docker-compose up -d
docker-compose logs postgres

# Si falla la migración
cd src/BusinessApp.Web
dotnet ef database drop --project ../BusinessApp.Infrastructure --force
dotnet ef migrations remove --project ../BusinessApp.Infrastructure
dotnet ef migrations add InitialCreate --project ../BusinessApp.Infrastructure
dotnet ef database update --project ../BusinessApp.Infrastructure

# Limpiar cache de NuGet
dotnet nuget locals all --clear
```

## Variables de Entorno (Producción)

```bash
# Configurar connection string
export ConnectionStrings__DefaultConnection="Host=prod-server;Port=5432;Database=businessapp;Username=prod_user;Password=secure_password"

# Configurar ambiente
export ASPNETCORE_ENVIRONMENT=Production
```

## Logs

Los logs se guardan en:
- **Consola:** Durante ejecución
- **Archivos:** `logs/businessapp-YYYYMMDD.txt`

Niveles de log:
- Debug (solo Development)
- Information
- Warning
- Error
- Critical
