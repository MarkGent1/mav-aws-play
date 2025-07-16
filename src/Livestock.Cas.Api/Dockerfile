# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS vsdebug
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["src/Livestock.Cas.Api/Livestock.Cas.Api.csproj", "Livestock.Cas.Api/"]
COPY ["src/Livestock.Cas.Application/Livestock.Cas.Application.csproj", "Livestock.Cas.Application/"]
COPY ["src/Livestock.Cas.Domain/Livestock.Cas.Domain.csproj", "Livestock.Cas.Domain/"]
COPY ["src/Livestock.Cas.Infrastructure/Livestock.Cas.Infrastructure.csproj", "Livestock.Cas.Infrastructure/"]

RUN dotnet restore "Livestock.Cas.Api/Livestock.Cas.Api.csproj" -r linux-x64 -v n
RUN dotnet restore "Livestock.Cas.Application/Livestock.Cas.Application.csproj" -r linux-x64 -v n
RUN dotnet restore "Livestock.Cas.Domain/Livestock.Cas.Domain.csproj" -r linux-x64 -v n
RUN dotnet restore "Livestock.Cas.Infrastructure/Livestock.Cas.Infrastructure.csproj" -r linux-x64 -v n

COPY ["src/", "."]

WORKDIR "Livestock.Cas.Api"
RUN dotnet publish "Livestock.Cas.Api.csproj" -v n -c Release -o /app -r linux-x64 --no-restore /p:Version=${VERSION}

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["./Livestock.Cas.Api"]