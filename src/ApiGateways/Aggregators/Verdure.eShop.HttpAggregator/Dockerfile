#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ApiGateways/Aggregators/Verdure.eShop.HttpAggregator/Verdure.eShop.HttpAggregator.csproj", "src/ApiGateways/Aggregators/Verdure.eShop.HttpAggregator/"]
RUN dotnet restore "src/ApiGateways/Aggregators/Verdure.eShop.HttpAggregator/Verdure.eShop.HttpAggregator.csproj"
COPY . .
WORKDIR "/src/src/ApiGateways/Aggregators/Verdure.eShop.HttpAggregator"
RUN dotnet build "Verdure.eShop.HttpAggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Verdure.eShop.HttpAggregator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Verdure.eShop.HttpAggregator.dll"]