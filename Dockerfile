# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["BackendApi.csproj", "./"]
RUN dotnet restore "BackendApi.csproj"

# Copy the rest of the files and build the app
# The .dockerignore ensures we don't copy bin/ or obj/ folders
COPY . .
RUN dotnet publish "BackendApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the ASP.NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/publish .

# Render requires the app to listen on a specific port.
# .NET 8 uses ASPNETCORE_HTTP_PORTS to define the listening port.
# Setting it to 8080 and making sure Render is configured for port 8080 is the most reliable way.
ENV ASPNETCORE_HTTP_PORTS=8080

# Explicitly set this environment variable for the connection string logic in Program.cs
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose the port (useful for documentation)
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "BackendApi.dll"]
