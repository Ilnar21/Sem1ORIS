# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory
WORKDIR /app

# Copy the project files
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /out

# Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0

# Set the working directory
WORKDIR /app

# Copy the built application
COPY --from=build /out .

# Expose port
EXPOSE 6529

# Start the application
ENTRYPOINT ["dotnet", "MyHTTPServer.dll"]
