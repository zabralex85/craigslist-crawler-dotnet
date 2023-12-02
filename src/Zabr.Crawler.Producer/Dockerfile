#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Zabr.Crawler.Producer/Zabr.Crawler.Producer.csproj", "Zabr.Crawler.Producer/"]
RUN dotnet restore "./Zabr.Crawler.Producer/./Zabr.Crawler.Producer.csproj"
COPY . .
WORKDIR "/src/Zabr.Crawler.Producer"
RUN dotnet build "./Zabr.Crawler.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Zabr.Crawler.Producer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zabr.Crawler.Producer.dll"]
