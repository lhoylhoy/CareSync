# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files for dependency restoration
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["src/CareSync.API/CareSync.API.csproj", "src/CareSync.API/"]
COPY ["src/CareSync.Application/CareSync.Application.csproj", "src/CareSync.Application/"]
COPY ["src/CareSync.Infrastructure/CareSync.Infrastructure.csproj", "src/CareSync.Infrastructure/"]
COPY ["src/CareSync.Domain/CareSync.Domain.csproj", "src/CareSync.Domain/"]

# Restore dependencies
RUN dotnet restore "src/CareSync.API/CareSync.API.csproj"

# Copy all source code
COPY . .

# Build and publish the application
WORKDIR "/src/src/CareSync.API"
RUN dotnet publish "CareSync.API.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Add metadata labels
LABEL maintainer="CareSync Team" \
    description="CareSync API - Healthcare Management System" \
    version="1.0"

WORKDIR /app

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published application
COPY --from=build /app/publish .

# Expose port (internal container port)
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set entrypoint
ENTRYPOINT ["dotnet", "CareSync.API.dll"]
