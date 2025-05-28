# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet build -c Release -o /app/build

# Publish stage  
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health check
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security (optional, bisa diaktifkan nanti)
# RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published app
COPY --from=publish /app/publish .

# Find and set the DLL name
RUN echo "#!/bin/sh" > /app/entrypoint.sh && \
    echo "DLL_FILE=\$(find /app -name '*.dll' -not -name '*.Views.dll' -not -name '*.PrecompiledViews.dll' | head -1)" >> /app/entrypoint.sh && \
    echo "if [ -z \"\$DLL_FILE\" ]; then" >> /app/entrypoint.sh && \
    echo "  echo 'No main DLL file found'" >> /app/entrypoint.sh && \
    echo "  exit 1" >> /app/entrypoint.sh && \
    echo "fi" >> /app/entrypoint.sh && \
    echo "echo \"Starting application: \$DLL_FILE\"" >> /app/entrypoint.sh && \
    echo "exec dotnet \"\$DLL_FILE\" \"\$@\"" >> /app/entrypoint.sh && \
    chmod +x /app/entrypoint.sh

# Change ownership to non-root user (optional)
# RUN chown -R appuser:appuser /app
# USER appuser

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["/app/entrypoint.sh"]