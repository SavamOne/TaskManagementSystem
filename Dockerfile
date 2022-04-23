FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ENV BuildingDocker true
WORKDIR /.
COPY /src /src
WORKDIR /src
RUN dotnet publish TaskManagementSystem.sln -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet TaskManagementSystem.Server.dll