# Etapa base com runtime + dependências extras
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Instala Docker CLI, docker-compose e bash
RUN apt-get update && apt-get install -y \
    docker.io \
    docker-compose \
    bash \
    && rm -rf /var/lib/apt/lists/*


WORKDIR /app
EXPOSE 80

# Etapa de build com SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar csproj
COPY src/Deploy.Api.Application/Deploy.Api.Application.csproj /src/Deploy.Api.Application/
COPY src/Deploy.Api.Core/Deploy.Api.Core.csproj /src/Deploy.Api.Core/

# Restaurar pacotes
RUN dotnet restore /src/Deploy.Api.Application/Deploy.Api.Application.csproj --verbosity detailed

# Copiar código
COPY src/Deploy.Api.Application/ /src/Deploy.Api.Application/
COPY src/Deploy.Api.Core/ /src/Deploy.Api.Core/

# Build
WORKDIR /src/Deploy.Api.Application
RUN dotnet build "Deploy.Api.Application.csproj" -c Release -o /app/build --verbosity detailed

# Publish
FROM build AS publish
RUN dotnet publish "Deploy.Api.Application.csproj" -c Release -o /app/publish

# Final com runtime + Docker CLI
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Deploy.Api.Application.dll"]
