#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Catalog/Verdure.eShop.Catalog.API/Verdure.eShop.Catalog.API.csproj", "src/Services/Catalog/Verdure.eShop.Catalog.API/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.csproj", "src/BuildingBlocks/EventBus/"]
COPY ["src/BuildingBlocks/Healthchecks/Healthchecks.csproj", "src/BuildingBlocks/Healthchecks/"]
RUN dotnet restore "src/Services/Catalog/Verdure.eShop.Catalog.API/Verdure.eShop.Catalog.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Catalog/Verdure.eShop.Catalog.API"
RUN dotnet build "Verdure.eShop.Catalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Verdure.eShop.Catalog.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Verdure.eShop.Catalog.API.dll"]