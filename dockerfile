# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file
COPY ["RescueSystem.slnx", "./"]

# Copy project files first for NuGet package restore caching
COPY ["src/RescueSystem.Api/RescueSystem.Api.csproj", "src/RescueSystem.Api/"]
COPY ["src/RescueSystem.Application/RescueSystem.Application.csproj", "src/RescueSystem.Application/"]
COPY ["src/RescueSystem.Domain/RescueSystem.Domain.csproj", "src/RescueSystem.Domain/"]
COPY ["src/RescueSystem.Infrastructure/RescueSystem.Infrastructure.csproj", "src/RescueSystem.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/RescueSystem.Api/RescueSystem.Api.csproj"

# Copy all remaining source files
COPY src/ src/

# Build and publish in Release mode
WORKDIR "/src/src/RescueSystem.Api"
RUN dotnet publish "RescueSystem.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-jammy-chiseled AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose HTTP port
EXPOSE 8080

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "RescueSystem.Api.dll"]