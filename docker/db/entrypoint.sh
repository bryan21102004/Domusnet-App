#!/bin/bash
set -e

/opt/mssql/bin/sqlservr &
PID=$!

echo "[DB] Waiting for SQL Server to start..."
for i in $(seq 1 30); do
    sleep 2
    if /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" -No 2>/dev/null; then
        echo "[DB] SQL Server is ready."
        break
    fi
    echo "[DB] Attempt $i/30 — not ready yet..."
done

if /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" \
    -Q "IF NOT EXISTS(SELECT 1 FROM sys.databases WHERE name='DomusNet') PRINT 'NOTEXISTS'" \
    -h -1 -No 2>/dev/null | grep -q "NOTEXISTS"; then

    echo "[DB] Initializing DomusNet database..."
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /docker-init/Tablas.sql -No
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /docker-init/Ajustes_DomusNet.sql -No
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /docker-init/PA-Auth.sql -No
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /docker-init/PA-Generales.sql -No
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -i /docker-init/ingresos.sql -No
    echo "[DB] Database initialized successfully."
else
    echo "[DB] Database already exists, skipping initialization."
fi

wait $PID
