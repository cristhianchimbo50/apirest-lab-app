# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo .csproj y restaurar dependencias
COPY ApiRest_LabWebApp/ApiRest_LabWebApp.csproj ./ApiRest_LabWebApp/
WORKDIR /src/ApiRest_LabWebApp
RUN dotnet restore

# Copiar todo el contenido del proyecto
COPY ApiRest_LabWebApp/. .

# Publicar la aplicación
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar los archivos publicados desde la etapa de build
COPY --from=build /app/publish .

# Exponer el puerto 80 (el mismo que en Kestrel)
EXPOSE 80

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "ApiRest_LabWebApp.dll"]
