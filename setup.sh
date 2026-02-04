#!/bin/bash

echo "🚀 BusinessApp - Setup Rápido"
echo "=============================="
echo ""

# Verificar Docker
if ! command -v docker &> /dev/null; then
    echo "❌ Docker no está instalado. Por favor instala Docker primero."
    exit 1
fi

# Verificar .NET
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK no está instalado. Por favor instala .NET 8 SDK primero."
    exit 1
fi

echo "✅ Docker y .NET detectados"
echo ""

# Levantar PostgreSQL
echo "📦 Levantando PostgreSQL..."
cd docker
docker compose up -d
cd ..

echo "⏳ Esperando a que PostgreSQL esté listo..."
sleep 5

# Restaurar paquetes
echo "📥 Restaurando paquetes NuGet..."
dotnet restore

# Navegar a Web
cd src/BusinessApp.Web

# Crear migración inicial si no existe
if [ ! -d "../BusinessApp.Infrastructure/Migrations" ]; then
    echo "🗃️  Creando migración inicial..."
    dotnet ef migrations add InitialCreate --project ../BusinessApp.Infrastructure
fi

# Aplicar migraciones
echo "🔄 Aplicando migraciones..."
dotnet ef database update --project ../BusinessApp.Infrastructure

echo ""
echo "✅ Setup completado!"
echo ""
echo "📌 Información importante:"
echo "   - PostgreSQL: localhost:5432"
echo "   - PgAdmin: http://localhost:5050"
echo "   - Usuario admin: admin@businessapp.com"
echo "   - Password: Admin123!"
echo ""
echo "🏃 Ejecuta 'dotnet run' para iniciar la aplicación"
