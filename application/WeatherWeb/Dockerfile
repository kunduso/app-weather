# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/WeatherWeb/WeatherWeb.csproj", "src/WeatherWeb/"]
RUN dotnet restore "src/WeatherWeb/WeatherWeb.csproj"

# Copy the rest of the code
COPY ["src/WeatherWeb/", "src/WeatherWeb/"]

# Build the application
WORKDIR "/src/src/WeatherWeb"
RUN dotnet build "WeatherWeb.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "WeatherWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN adduser --disabled-password --gecos "" appuser && \
    chown -R appuser:appuser /app
USER appuser

# Add healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8081/healthcheck || exit 1

EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "WeatherWeb.dll"]
