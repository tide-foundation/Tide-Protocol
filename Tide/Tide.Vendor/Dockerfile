FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.14 AS base
RUN  apk add --no-cache bash
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.13 AS build
WORKDIR /app/publish/
RUN apk add --no-cache nodejs npm \
    && curl -sSL https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/publish/wait-for-it.sh \
    && chmod +x /app/publish/wait-for-it.sh

WORKDIR /src/Tide
COPY ["Tide.Core/Tide.Core.csproj", "Tide.Core/"]
COPY ["Tide.VendorSdk/Tide.VendorSdk.csproj", "Tide.VendorSdk/"]
COPY ["Tide.Vendor/Tide.Vendor.csproj", "Tide.Vendor/"]
COPY ["Tide.Vendor/Client/package.json", "Tide.Vendor/Client/package-lock.json", "Tide.Vendor/Client/"]
RUN dotnet restore "Tide.Vendor/Tide.Vendor.csproj" \
    && npm install --cwd ./Tide.Vendor/Client --prefix ./Tide.Vendor/Client

COPY ["Tide.Core", "Tide.Core/"]
COPY ["Tide.VendorSdk", "Tide.VendorSdk/"]
RUN dotnet build "Tide.VendorSdk" -c Release

COPY ["Tide.Vendor", "Tide.Vendor/"]
RUN dotnet publish "Tide.Vendor/Tide.Vendor.csproj" -c Release -o /app/publish /p:EnvironmentName=production

FROM base AS final
COPY --from=build /app/publish .
CMD ["dotnet", "Tide.Vendor"]
