FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/WeatherApi/WeatherApi.csproj", "src/WeatherApi/"]
RUN dotnet restore "src/WeatherApi/WeatherApi.csproj"

# Copy the rest of the code
COPY ["src/WeatherApi/", "src/WeatherApi/"]

# Build the application
WORKDIR "/src/src/WeatherApi"
RUN dotnet build "WeatherApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "WeatherApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "WeatherApi.dll"]
