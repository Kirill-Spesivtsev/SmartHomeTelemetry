#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DataProcessor/DataProcessor.Api/DataProcessor.Api.csproj", "DataProcessor/DataProcessor.Api/"]
COPY ["DataProcessor/DataProcessor.Infrastructure/DataProcessor.Infrastructure.csproj", "DataProcessor/DataProcessor.Infrastructure/"]
COPY ["DataProcessor/DataProcessor.Domain/DataProcessor.Domain.csproj", "DataProcessor/DataProcessor.Domain/"]
COPY ["SmartHomeTelemetry.Shared/SmartHomeTelemetry.Shared.csproj", "SmartHomeTelemetry.Shared/"]
RUN dotnet restore "./DataProcessor/DataProcessor.Api/DataProcessor.Api.csproj"
COPY . .
WORKDIR "/src/DataProcessor/DataProcessor.Api"
RUN dotnet build "./DataProcessor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DataProcessor.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataProcessor.Api.dll"]