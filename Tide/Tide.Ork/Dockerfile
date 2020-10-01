FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
RUN apt update -y && apt install nano -y
WORKDIR /src
COPY ["Tide/Tide.Ork/Tide.Ork.csproj", "Tide/Tide.Ork/"]
COPY ["Tide/Tide.Core/Tide.Core.csproj", "Tide/Tide.Core/"]
COPY ["Tide/Tide.VendorSdk/Tide.VendorSdk.csproj", "Tide/Tide.VendorSdk/"]
RUN dotnet restore "Tide/Tide.Ork/Tide.Ork.csproj"

COPY ["Tide/Tide.Core/", "Tide/Tide.Core/"]
COPY ["Tide/Tide.VendorSdk", "Tide/Tide.VendorSdk"]
RUN dotnet build "Tide/Tide.VendorSdk" -c Release

COPY ["Tide/Tide.Ork", "Tide/Tide.Ork"]
WORKDIR "/src/Tide/Tide.Ork"
RUN dotnet publish "Tide.Ork.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Tide.Ork.dll"]
