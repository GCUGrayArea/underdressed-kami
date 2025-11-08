# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY SmartScheduler.Domain/SmartScheduler.Domain.csproj SmartScheduler.Domain/
COPY SmartScheduler.Application/SmartScheduler.Application.csproj SmartScheduler.Application/
COPY SmartScheduler.Infrastructure/SmartScheduler.Infrastructure.csproj SmartScheduler.Infrastructure/
COPY SmartScheduler.WebApi/SmartScheduler.WebApi.csproj SmartScheduler.WebApi/

# Restore dependencies (WebApi project references all others)
RUN dotnet restore SmartScheduler.WebApi/SmartScheduler.WebApi.csproj

# Copy source code
COPY SmartScheduler.Domain/ SmartScheduler.Domain/
COPY SmartScheduler.Application/ SmartScheduler.Application/
COPY SmartScheduler.Infrastructure/ SmartScheduler.Infrastructure/
COPY SmartScheduler.WebApi/ SmartScheduler.WebApi/

# Build and publish
WORKDIR /src/SmartScheduler.WebApi
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup -g 1000 appuser && \
    adduser -u 1000 -G appuser -s /bin/sh -D appuser

# Copy published app
COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "SmartScheduler.WebApi.dll"]
