# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# remove a porta padrão automática
ENV ASPNETCORE_HTTP_PORTS=""

# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Deploy.Api.Application/Deploy.Api.Application.csproj Deploy.Api.Application/
COPY src/Deploy.Api.Core/Deploy.Api.Core.csproj Deploy.Api.Core/

RUN dotnet restore Deploy.Api.Application/Deploy.Api.Application.csproj

COPY src/Deploy.Api.Application/ Deploy.Api.Application/
COPY src/Deploy.Api.Core/ Deploy.Api.Core/

WORKDIR /src/Deploy.Api.Application
RUN dotnet publish -c Release -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Deploy.Api.Application.dll"]
