# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet build backendcafe.csproj -c Release -o /app/build

# Publish stage  
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl and postgresql-client for health check and wait-for-it
RUN apt-get update && apt-get install -y curl postgresql-client && rm -rf /var/lib/apt/lists/*

# Create non-root user for security (optional, bisa diaktifkan nanti)
# RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published app
COPY --from=publish /app/publish .

# Find and set the main application DLL name, and add wait-for-it logic
RUN echo "#!/bin/sh" > /app/entrypoint.sh && \
    echo "# Wait for database to be ready" >> /app/entrypoint.sh && \
    echo "until pg_isready -h db -p 5432 -U ${DB_USER:-postgres} -d ${DB_NAME:-cafemobile}; do" >> /app/entrypoint.sh && \
    echo "  echo 'Menunggu database siap...'" >> /app/entrypoint.sh && \
    echo "  sleep 2" >> /app/entrypoint.sh && \
    echo "done" >> /app/entrypoint.sh && \
    echo "echo 'Database siap!'" >> /app/entrypoint.sh && \
    echo "# Find main application DLL (has runtimeconfig.json)" >> /app/entrypoint.sh && \
    echo "DLL_FILE=\$(find /app -name '*.runtimeconfig.json' | sed 's/.runtimeconfig.json/.dll/')" >> /app/entrypoint.sh && \
    echo "if [ -z \"\$DLL_FILE\" ] || [ ! -f \"\$DLL_FILE\" ]; then" >> /app/entrypoint.sh && \
    echo "  echo 'Searching for main DLL by excluding known library DLLs...'" >> /app/entrypoint.sh && \
    echo "  DLL_FILE=\$(find /app -maxdepth 1 -name '*.dll' -not -name 'Microsoft.*' -not -name 'System.*' -not -name 'Swashbuckle.*' -not -name 'Newtonsoft.*' -not -name 'Npgsql.*' -not -name '*.Views.dll' | head -1)" >> /app/entrypoint.sh && \
    echo "fi" >> /app/entrypoint.sh && \
    echo "if [ -z \"\$DLL_FILE\" ] || [ ! -f \"\$DLL_FILE\" ]; then" >> /app/entrypoint.sh && \
    echo "  echo 'ERROR: No main application DLL found'" >> /app/entrypoint.sh && \
    echo "  echo 'Available DLL files:'" >> /app/entrypoint.sh && \
    echo "  ls -la /app/*.dll" >> /app/entrypoint.sh && \
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