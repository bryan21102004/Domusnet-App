FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DomusNet.Shared/DomusNet.Shared.csproj DomusNet.Shared/
COPY DomusNet.API/DomusNet.API.csproj       DomusNet.API/
RUN dotnet restore DomusNet.API/DomusNet.API.csproj

COPY DomusNet.Shared/ DomusNet.Shared/
COPY DomusNet.API/    DomusNet.API/
RUN dotnet publish DomusNet.API/DomusNet.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "DomusNet.API.dll"]
