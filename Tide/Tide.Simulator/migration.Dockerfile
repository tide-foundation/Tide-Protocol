FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine3.13 AS base

ARG MSSQL_VERSION=17.5.2.1-1
ENV MSSQL_VERSION=${MSSQL_VERSION}
ENV PATH="${PATH}:/root/.dotnet/tools:/opt/mssql-tools/bin"

WORKDIR /tmp
RUN apk add --no-cache curl gnupg netcat-openbsd nano --virtual .build-dependencies -- \
    # Adding custom MS repository for mssql-tools and msodbcsql
    && curl -O https://download.microsoft.com/download/e/4/e/e4e67866-dffd-428c-aac7-8d28ddafb39b/msodbcsql17_${MSSQL_VERSION}_amd64.apk \
    && curl -O https://download.microsoft.com/download/e/4/e/e4e67866-dffd-428c-aac7-8d28ddafb39b/mssql-tools_${MSSQL_VERSION}_amd64.apk \
    # Verifying signature
    && curl -O https://download.microsoft.com/download/e/4/e/e4e67866-dffd-428c-aac7-8d28ddafb39b/msodbcsql17_${MSSQL_VERSION}_amd64.sig \
    && curl -O https://download.microsoft.com/download/e/4/e/e4e67866-dffd-428c-aac7-8d28ddafb39b/mssql-tools_${MSSQL_VERSION}_amd64.sig \
    # Adding wait-for-it.sh
    && mkdir /app \
    && curl -sSL https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/wait-for-it.sh \
    && chmod +x /app/wait-for-it.sh \
    # Importing gpg key
    && curl https://packages.microsoft.com/keys/microsoft.asc  | gpg --import - \
    && gpg --verify msodbcsql17_${MSSQL_VERSION}_amd64.sig msodbcsql17_${MSSQL_VERSION}_amd64.apk \
    && gpg --verify mssql-tools_${MSSQL_VERSION}_amd64.sig mssql-tools_${MSSQL_VERSION}_amd64.apk \
    # Installing packages
    && echo y | apk add --allow-untrusted msodbcsql17_${MSSQL_VERSION}_amd64.apk mssql-tools_${MSSQL_VERSION}_amd64.apk \
    # Deleting packages
    && apk del .build-dependencies && rm -f msodbcsql*.sig mssql-tools*.apk \
    # installing dotnet ef
    && apk add --no-cache bash \
    && dotnet new tool-manifest \
    && dotnet tool install --global dotnet-ef

WORKDIR /app
COPY ["Tide.Core/", "Tide.Core/"]
COPY ["Tide.Simulator/", "Tide.Simulator/"]
RUN chmod +x "./Tide.Simulator/entrypoint.sh" \
    && dotnet-ef migrations script --idempotent --project "Tide.Simulator" --output "migration.sql"
RUN sed -i '1s/^.//' migration.sql \
    && sed -i "1s/^/CREATE DATABASE [db];\nGO\n\nUSE db;\nGO\n\n/" migration.sql \
    && sed -i "1s/^/IF NOT EXISTS(SELECT name FROM sys.sysdatabases where name='db')\n/" migration.sql

CMD ["./Tide.Simulator/entrypoint.sh"]
