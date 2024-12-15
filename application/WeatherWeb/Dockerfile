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
EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "WeatherWeb.dll"]
