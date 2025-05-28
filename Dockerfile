# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health check
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Create entrypoint script that finds the main assembly
RUN echo '#!/bin/bash\n\
set -e\n\
\n\
# Debug: List all files\n\
echo "=== DEBUG: Files in /app ==="\n\
ls -la /app/\n\
echo ""\n\
\n\
# Find the main application DLL by looking for runtimeconfig.json\n\
MAIN_DLL=""\n\
for config in /app/*.runtimeconfig.json; do\n\
    if [ -f "$config" ]; then\n\
        dll_name=$(basename "$config" .runtimeconfig.json).dll\n\
        if [ -f "/app/$dll_name" ]; then\n\
            MAIN_DLL="$dll_name"\n\
            break\n\
        fi\n\
    fi\n\
done\n\
\n\
# If not found, try to find by excluding common library DLLs\n\
if [ -z "$MAIN_DLL" ]; then\n\
    echo "No runtimeconfig.json found, searching for main DLL..."\n\
    for dll in /app/*.dll; do\n\
        dll_name=$(basename "$dll")\n\
        # Skip known library DLLs\n\
        if [[ ! "$dll_name" =~ ^(Microsoft\.|System\.|Swashbuckle\.|Newtonsoft\.|Npgsql\.|.*\.Views\.dll$) ]]; then\n\
            MAIN_DLL="$dll_name"\n\
            break\n\
        fi\n\
    done\n\
fi\n\
\n\
if [ -z "$MAIN_DLL" ]; then\n\
    echo "ERROR: Could not find main application DLL"\n\
    echo "Available DLL files:"\n\
    ls -la /app/*.dll\n\
    exit 1\n\
fi\n\
\n\
echo "Starting application: $MAIN_DLL"\n\
exec dotnet "/app/$MAIN_DLL" "$@"' > /app/entrypoint.sh && \
    chmod +x /app/entrypoint.sh

# Health check  
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["/app/entrypoint.sh"]