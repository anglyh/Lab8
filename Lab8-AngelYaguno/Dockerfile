# Usar la imagen base de .NET 8 SDK para construir
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar el archivo csproj y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el resto del código
COPY . ./
RUN dotnet publish -c Release -o out

# Generar imagen de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Crear usuario no root por seguridad
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Exponer el puerto (Render usa el puerto de la variable PORT)
EXPOSE 8080
ENV PORT=8080

# Configurar la aplicación para escuchar en todos los interfaces
ENV ASPNETCORE_URLS=http://*:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Lab8-AngelYaguno.dll"] 