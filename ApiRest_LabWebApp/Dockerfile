# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el .csproj (usando ruta relativa al contexto raíz)
COPY ApiRest_LabWebApp/ApiRest_LabWebApp.csproj ./ApiRest_LabWebApp/
WORKDIR /src/ApiRest_LabWebApp
RUN dotnet restore "ApiRest_LabWebApp.csproj"

# Copiar el resto del proyecto
COPY ApiRest_LabWebApp/. .

# Compilar y publicar
RUN dotnet publish "ApiRest_LabWebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/publish .

# Exponer el puerto
EXPOSE 80

# Comando de arranque
ENTRYPOINT ["dotnet", "ApiRest_LabWebApp.dll"]
