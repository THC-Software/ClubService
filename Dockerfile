FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ClubService.API/ClubService.API.csproj", "ClubService.API/"]
COPY ["ClubService.Application/ClubService.Application.csproj", "ClubService.Application/"]
COPY ["ClubService.Domain/ClubService.Domain.csproj", "ClubService.Domain/"]
COPY ["ClubService.Infrastructure/ClubService.Infrastructure.csproj", "ClubService.Infrastructure/"]
RUN dotnet restore "ClubService.API/ClubService.API.csproj"
COPY . .
WORKDIR "/src/ClubService.API"
RUN dotnet build "ClubService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ClubService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV POSTGRES_HOST=localhost
ENV POSTGRES_PORT=5432
ENV POSTGRES_USER=user
ENV POSTGRES_PASSWORD=password
ENV EVENT_STORE_DB=club-service-event-store
ENV READ_STORE_DB=club-service-read-store
ENV LOGIN_STORE_DB=club-service-login-store
ENV REDIS_HOST=localhost
ENV REDIS_PORT=6379
ENV REDIS_POLLING_INTERVAL=1
ENV SMTP_HOST=localhost
ENV SMTP_PORT=1025
ENV SMTP_SENDER_EMAIL_ADDRESS=admin@thcdornbirn.at
ENV EMAIL_OUTBOX_POLLING_INTERVAL=10

ENTRYPOINT ["dotnet", "ClubService.API.dll"]
