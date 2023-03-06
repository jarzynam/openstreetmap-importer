#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./", "./"]
RUN dotnet restore "src/OpenStreetMap.Importer/OpenStreetMap.Importer.csproj"
COPY . .
WORKDIR "/OpenStreetMap.Importer"
RUN dotnet build "OpenStreetMap.Importer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OpenStreetMap.Importer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenStreetMap.Importer.dll"]