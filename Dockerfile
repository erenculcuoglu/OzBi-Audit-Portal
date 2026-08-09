# Multi-stage production Dockerfile for OzBI Portal CRM (.NET 8 Blazor Server)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["OzBiPortalCRM.csproj", "./"]
RUN dotnet restore "OzBiPortalCRM.csproj"

# Copy full source and publish
COPY . .
RUN dotnet publish "OzBiPortalCRM.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create directory for SQLite database
RUN mkdir -p /app/app && chmod -R 777 /app/app

COPY --from=build /app/publish .

ENV PORT=8080
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE 8080

ENTRYPOINT ["dotnet", "OzBiPortalCRM.dll"]
