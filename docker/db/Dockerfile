FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

RUN mkdir -p /docker-init

COPY docker/db/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

COPY DomusNet.API/Data/Tablas.sql           /docker-init/Tablas.sql
COPY DomusNet.API/Data/Ajustes_DomusNet.sql /docker-init/Ajustes_DomusNet.sql
COPY DomusNet.API/Data/PA-Auth.sql          /docker-init/PA-Auth.sql
COPY DomusNet.API/Data/PA-Generales.sql     /docker-init/PA-Generales.sql
COPY DomusNet.API/Data/ingresos.sql         /docker-init/ingresos.sql

USER mssql

ENTRYPOINT ["/entrypoint.sh"]
