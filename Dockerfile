# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OzBI Portal CRM.csproj", "./"]
RUN dotnet restore "OzBI Portal CRM.csproj"
COPY . .
RUN dotnet publish "OzBI Portal CRM.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "OzBiPortalCRM.dll"]
