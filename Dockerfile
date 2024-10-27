# Use the SDK image for building and running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install curl for debugging
RUN apt-get update && apt-get install -y curl

# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroServiceMicrocontrollerManager.csproj", "."]
RUN dotnet restore "./MicroServiceMicrocontrollerManager.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./MicroServiceMicrocontrollerManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./MicroServiceMicrocontrollerManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy appsettings files
COPY appsettings.json .
COPY appsettings.Development.json .
COPY appsettings.Docker.json .

ENTRYPOINT ["dotnet", "MicroServiceMicrocontrollerManager.dll"]