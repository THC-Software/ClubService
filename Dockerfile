FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ClubService.API/ClubService.API.csproj", "ClubService.API/"]
COPY ["src/ClubService.Application/ClubService.Application.csproj", "ClubService.Application/"]
RUN dotnet restore "ClubService.API/ClubService.API.csproj"
COPY src .
WORKDIR "/src/ClubService.API"
RUN dotnet build "ClubService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ClubService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClubService.API.dll"]
