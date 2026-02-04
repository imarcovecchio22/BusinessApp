# BusinessApp - Sistema de Gestión para PyMEs

Sistema de gestión empresarial desarrollado con .NET 8 y PostgreSQL, siguiendo arquitectura limpia (Clean Architecture).

## 🏗️ Estructura del Proyecto

```
BusinessApp/
├── src/
│   ├── BusinessApp.Web/           # Capa de Presentación (Razor Pages, Controllers)
│   ├── BusinessApp.Application/   # Capa de Aplicación (DTOs, Interfaces, Lógica)
│   ├── BusinessApp.Domain/        # Capa de Dominio (Entidades, Value Objects)
│   └── BusinessApp.Infrastructure/# Capa de Infraestructura (EF Core, Servicios)
└── docker/
    └── docker-compose.yml         # PostgreSQL + PgAdmin
```

## 🚀 Setup Inicial

### Prerequisitos

- .NET 8 SDK
- Docker y Docker Compose
- IDE (Visual Studio, Rider, o VS Code)

### Paso 1: Levantar PostgreSQL

```bash
cd docker
docker-compose up -d
```

Esto levanta:
- PostgreSQL en `localhost:5432`
- PgAdmin en `http://localhost:5050` (usuario: admin@businessapp.com, pass: admin)

### Paso 2: Restaurar paquetes NuGet

```bash
cd ..
dotnet restore
```

### Paso 3: Crear la primera migración

```bash
cd src/BusinessApp.Web
dotnet ef migrations add InitialCreate --project ../BusinessApp.Infrastructure
```

### Paso 4: Aplicar migraciones y ejecutar

```bash
dotnet run
```

La aplicación se ejecutará en:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5001`

## 👤 Usuario Inicial

Al ejecutar por primera vez, se crea un usuario administrador:

- **Email:** admin@businessapp.com
- **Password:** Admin123!

## 🔐 Roles del Sistema

1. **Admin** - Acceso completo al sistema
2. **Usuario** - Operaciones básicas
3. **Contador** - Gestión financiera

## 📦 Tecnologías

- **.NET 8** - Framework principal
- **PostgreSQL 16** - Base de datos
- **Entity Framework Core 8** - ORM
- **ASP.NET Identity** - Autenticación y autorización
- **Serilog** - Logging estructurado
- **Razor Pages** - Frontend

## 🗂️ Comandos Útiles

### Migraciones

```bash
# Crear nueva migración
dotnet ef migrations add MigrationName --project src/BusinessApp.Infrastructure --startup-project src/BusinessApp.Web

# Aplicar migraciones
dotnet ef database update --project src/BusinessApp.Infrastructure --startup-project src/BusinessApp.Web

# Eliminar última migración
dotnet ef migrations remove --project src/BusinessApp.Infrastructure --startup-project src/BusinessApp.Web
```

### Docker

```bash
# Iniciar servicios
docker-compose up -d

# Detener servicios
docker-compose down

# Ver logs
docker-compose logs -f postgres
```

## 📝 Logs

Los logs se guardan en:
- Consola (durante desarrollo)
- `logs/businessapp-YYYYMMDD.txt`

## 🎯 Próximos Pasos (Fase 2)

- [ ] Módulo de Clientes
- [ ] Módulo de Productos/Servicios
- [ ] Dashboard con métricas
- [ ] Sistema de categorías

## 📄 Licencia

Proyecto privado para uso comercial.
