FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /.
COPY /src /src
RUN dotnet publish src/TaskManagementSystem.sln -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet TaskManagementSystem.Server.dll