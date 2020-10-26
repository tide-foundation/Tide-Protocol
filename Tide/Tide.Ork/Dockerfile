FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
RUN apt update -y && apt install nano curl -y \
    && curl -sL https://deb.nodesource.com/setup_12.x -o nodesource_setup.sh \
    && bash nodesource_setup.sh \
    && apt install nodejs

WORKDIR /src
COPY ["Tide/Tide.Ork/Tide.Ork.csproj", "Tide/Tide.Ork/"]
COPY ["Tide/Tide.Core/Tide.Core.csproj", "Tide/Tide.Core/"]
COPY ["Tide/Tide.VendorSdk/Tide.VendorSdk.csproj", "Tide/Tide.VendorSdk/"]
COPY ["Tide/Tide.Js/package.json", "Tide/Tide.Js/package-lock.json", "Tide/Tide.Js/"]
COPY ["Tide/Tide.Ork/Client/package.json", "Tide/Tide.Ork/Client/package-lock.json", "Tide/Tide.Ork/Client/"]
RUN dotnet restore "Tide/Tide.Ork/Tide.Ork.csproj" \
    && npm install --cwd /src/Tide/Tide.Js --prefix /src/Tide/Tide.Js \
    && npm install --cwd /src/Tide/Tide.Ork/Client --prefix /src/Tide/Tide.Ork/Client

COPY ["Tide/Tide.Core/", "Tide/Tide.Core/"]
COPY ["Tide/Tide.VendorSdk", "Tide/Tide.VendorSdk"]
COPY ["Tide/Tide.Js/src", "Tide/Tide.Js/src"]
RUN dotnet build "Tide/Tide.VendorSdk" -c Release

COPY ["Tide/Tide.Ork", "Tide/Tide.Ork"]
WORKDIR "/src/Tide/Tide.Ork"
RUN dotnet publish "Tide.Ork.csproj" -c Release -o /app/publish \
    && rm -fr /app/publish/Client/node_modules /app/publish/Client/public

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tide.Ork.dll"]
