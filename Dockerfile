# Etapa base com runtime apenas
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Etapa de build com SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os arquivos de projeto para o diretório correto dentro do container
COPY src/Deploy.Api.Application/Deploy.Api.Application.csproj /src/Deploy.Api.Application/
COPY src/Deploy.Api.Core/Deploy.Api.Core.csproj /src/Deploy.Api.Core/

# Garantir que o NuGet está apontando para o repositório correto
RUN dotnet nuget list source | grep -q "nuget.org" || dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# Restaurar as dependências
RUN dotnet restore /src/Deploy.Api.Application/Deploy.Api.Application.csproj --verbosity detailed

# Copiar o restante do código fonte
COPY src/Deploy.Api.Application/ /src/Deploy.Api.Application/
COPY src/Deploy.Api.Core/ /src/Deploy.Api.Core/

# Build da aplicação
WORKDIR /src/Deploy.Api.Application
RUN dotnet build "Deploy.Api.Application.csproj" -c Release -o /app/build --verbosity detailed

# Publish da aplicação
FROM build AS publish
RUN dotnet publish "Deploy.Api.Application.csproj" -c Release -o /app/publish

# Imagem final com runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Deploy.Api.Application.dll"]
