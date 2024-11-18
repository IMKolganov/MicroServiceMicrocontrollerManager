# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Установка зависимостей и сборка проекта
WORKDIR /src
COPY ["MicroServiceMicrocontrollerManager.csproj", "."]
RUN dotnet restore "MicroServiceMicrocontrollerManager.csproj"
COPY . .
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "MicroServiceMicrocontrollerManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикация проекта
FROM build AS publish
RUN dotnet publish "MicroServiceMicrocontrollerManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный слой с использованием пользователя app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
RUN apt-get update && apt-get install -y curl

# Проверка и создание пользователя app, если он отсутствует
RUN if ! id -u app > /dev/null 2>&1; then useradd -m app; fi
RUN mkdir -p /app && chown -R app:app /app

USER app
WORKDIR /app
COPY --from=publish /app/publish .

# Копируем конфигурационные файлы
COPY appsettings.json .
COPY appsettings.Development.json .
COPY appsettings.Docker.json .

# Указываем команду для запуска
ENTRYPOINT ["dotnet", "MicroServiceMicrocontrollerManager.dll"]