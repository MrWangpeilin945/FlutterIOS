# syntax=docker/dockerfile:1

# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY Backend/src/Wellship.WebAPI ./Wellship.WebAPI
COPY Backend/src/Wellship.Core ./Wellship.Core
COPY Backend/src/Wellship.APIModels ./Wellship.APIModels
RUN dotnet restore "Wellship.WebAPI/Wellship.WebAPI.csproj"
RUN dotnet publish "Wellship.WebAPI/Wellship.WebAPI.csproj" -c Release -o /app --no-restore

# final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app ./

ENV TZ=Asia/Tokyo
ENV ASPNETCORE_HTTP_PORTS=80

RUN groupadd -r appuser && useradd --no-log-init -r -g appuser appuser
RUN chown -R appuser:appuser /app

USER appuser

ENTRYPOINT ["dotnet", "Wellship.WebAPI.dll"]
