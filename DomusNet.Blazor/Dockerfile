FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY DomusNet.Shared/DomusNet.Shared.csproj DomusNet.Shared/
COPY DomusNet.Blazor/DomusNet.Blazor.csproj DomusNet.Blazor/
RUN dotnet restore DomusNet.Blazor/DomusNet.Blazor.csproj

COPY DomusNet.Shared/ DomusNet.Shared/
COPY DomusNet.Blazor/ DomusNet.Blazor/
RUN dotnet publish DomusNet.Blazor/DomusNet.Blazor.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
COPY --from=build /app/publish/wwwroot    /usr/share/nginx/html
COPY DomusNet.Blazor/nginx.conf           /etc/nginx/conf.d/default.conf
EXPOSE 80
