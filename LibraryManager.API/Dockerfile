# ESTÁGIO 1: Base para rodar a aplicação (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ESTÁGIO 2: Build da aplicação (SDK - a parte pesada)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de projeto e restaura as dependências (otimiza o cache do Docker)
COPY ["LibraryManager.API.csproj", "./"]
RUN dotnet restore "./LibraryManager.API.csproj"

# Copia o restante dos arquivos e compila
COPY . .
RUN dotnet build "LibraryManager.API.csproj" -c Release -o /app/build

# ESTÁGIO 3: Publicação (Otimização)
FROM build AS publish
RUN dotnet publish "LibraryManager.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ESTÁGIO 4: Imagem Final (Leve e pronta para o GitHub)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibraryManager.API.dll"]