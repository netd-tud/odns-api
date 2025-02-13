# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore

# Build and publish the application
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the compiled application from the build stage
COPY --from=build /app/publish ./

# Expose the application's port
EXPOSE 5551

# Set the entry point for the application
ENTRYPOINT ["dotnet", "ODNSAPI.dll"]
