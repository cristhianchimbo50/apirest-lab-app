# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solo el .csproj y restaurar dependencias primero
COPY ["ApiRest_LabWebApp/ApiRest_LabWebApp.csproj", "./"]
RUN dotnet restore "ApiRest_LabWebApp.csproj"

# Copiar todo y compilar
COPY . .
RUN dotnet publish "ApiRest_LabWebApp.csproj" -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=build /app/publish .

# Exponer el puerto
EXPOSE 80

# Comando de arranque
ENTRYPOINT ["dotnet", "ApiRest_LabWebApp.dll"]
