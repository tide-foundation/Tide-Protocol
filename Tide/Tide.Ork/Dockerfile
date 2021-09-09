FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine3.13 AS base
RUN  apk add --no-cache bash
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine3.13 as build
WORKDIR /app/publish/
RUN apk add --no-cache nodejs npm curl bash \
    && curl -sSL https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/publish/wait-for-it.sh \
    && chmod +x /app/publish/wait-for-it.sh

WORKDIR /src/Tide
COPY ["Tide.Ork/Tide.Ork.csproj", "Tide.Ork/"]
COPY ["Tide.Core/Tide.Core.csproj", "Tide.Core/"]
COPY ["Tide.VendorSdk/Tide.VendorSdk.csproj", "Tide.VendorSdk/"]
COPY ["Tide.Js/package.json", "Tide.Js/package-lock.json", "Tide.Js/"]
COPY ["Tide.Ork/Enclave/package.json", "Tide.Ork/Enclave/package-lock.json", "Tide.Ork/Enclave/"]
RUN dotnet restore "Tide.Ork/Tide.Ork.csproj" \
    && npm install --cwd ./Tide.Js --prefix ./Tide.Js \
    && npm install --cwd ./Tide.Ork/Enclave --prefix ./Tide.Ork/Enclave

COPY ["Tide.Core/", "Tide.Core/"]
COPY ["Tide.VendorSdk", "Tide.VendorSdk"]
COPY ["Tide.Js/src", "Tide.Js/src"]
RUN dotnet build "Tide.VendorSdk" -c Release

COPY ["Tide.Ork", "Tide.Ork"]
RUN dotnet publish "Tide.Ork/Tide.Ork.csproj" -c Release -o /app/publish \
    && rm -fr /app/publish/Enclave/node_modules /app/publish/Enclave/public

FROM base AS final
COPY --from=build /app/publish .
CMD ["dotnet", "Tide.Ork.dll"]
