FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM tideorg/dotnet-node-sdk:3-12 AS build
WORKDIR /app/publish/
RUN curl -sSL https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/publish/wait-for-it.sh \
    && chmod +x /app/publish/wait-for-it.sh

WORKDIR /src/Tide
COPY ["Tide.Ork/Tide.Ork.csproj", "Tide.Ork/"]
COPY ["Tide.Core/Tide.Core.csproj", "Tide.Core/"]
COPY ["Tide.VendorSdk/Tide.VendorSdk.csproj", "Tide.VendorSdk/"]
COPY ["Tide.Js/package.json", "Tide.Js/package-lock.json", "Tide.Js/"]
COPY ["Tide.Ork/Client/package.json", "Tide.Ork/Client/package-lock.json", "Tide.Ork/Client/"]
RUN dotnet restore "Tide.Ork/Tide.Ork.csproj" \
    && npm install --cwd ./Tide.Js --prefix ./Tide.Js \
    && npm install --cwd ./Tide.Ork/Client --prefix ./Tide.Ork/Client

COPY ["Tide.Core/", "Tide.Core/"]
COPY ["Tide.VendorSdk", "Tide.VendorSdk"]
COPY ["Tide.Js/src", "Tide.Js/src"]
RUN dotnet build "Tide.VendorSdk" -c Release

COPY ["Tide.Ork", "Tide.Ork"]
RUN dotnet publish "Tide.Ork/Tide.Ork.csproj" -c Release -o /app/publish \
    && rm -fr /app/publish/Client/node_modules /app/publish/Client/public

FROM base AS final
COPY --from=build /app/publish .
CMD ["dotnet", "Tide.Ork.dll"]
