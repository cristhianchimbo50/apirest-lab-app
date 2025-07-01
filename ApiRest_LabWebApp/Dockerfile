# Imagen base de ASP.NET Core para producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Imagen base para compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "ApiRest_LabWebApp.csproj"
RUN dotnet build "ApiRest_LabWebApp.csproj" -c Release -o /app/build
RUN dotnet publish "ApiRest_LabWebApp.csproj" -c Release -o /app/publish

# Publicación final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiRest_LabWebApp.dll"]
