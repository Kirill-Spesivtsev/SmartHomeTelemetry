# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiGateway/ApiGateway.Api/ApiGateway.Api.csproj", "ApiGateway/ApiGateway.Api/"]
COPY ["ApiGateway/ApiGateway.Infrastructure/ApiGateway.Infrastructure.csproj", "ApiGateway.Infrastructure/"]
COPY ["ApiGateway/ApiGateway.Application/ApiGateway.Application.csproj", "ApiGateway/ApiGateway.Application/"]
COPY ["ApiGateway/ApiGateway.Domain/ApiGateway.Domain.csproj", "ApiGateway/ApiGateway.Domain/"]
COPY ["DataProcessor/DataProcessor.Domain/DataProcessor.Domain.csproj", "DataProcessor/DataProcessor.Domain/"]
COPY ["SmartHomeTelemetry.Shared/SmartHomeTelemetry.Shared.csproj", "SmartHomeTelemetry.Shared/"]
RUN dotnet restore "./ApiGateway/ApiGateway.Api/ApiGateway.Api.csproj"
COPY . .
WORKDIR "/src/ApiGateway/ApiGateway.Api"
RUN dotnet build "./ApiGateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ApiGateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.Api.dll"]