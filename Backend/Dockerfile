FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY CarteiraPro.slnx ./
COPY API/API.csproj API/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/

RUN dotnet restore API/API.csproj

COPY . .
RUN dotnet publish API/API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
HEALTHCHECK --interval=10s --timeout=5s --retries=10 --start-period=20s \
    CMD curl -f http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "API.dll"]
