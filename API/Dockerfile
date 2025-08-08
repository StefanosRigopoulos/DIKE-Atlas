# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the API project
COPY API/*.csproj ./API/
RUN dotnet restore ./API/API.csproj
COPY . ./

# Publish the .NET application
WORKDIR /app/API
RUN dotnet publish -c Release -o /app/publish

# Use the runtime image to serve the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Copy the published application
COPY --from=build /app/publish .

# Set the environment variable for Kestrel
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "API.dll"]