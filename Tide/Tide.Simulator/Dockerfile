FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app/publish/
RUN curl -sSL https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh > /app/publish/wait-for-it.sh \
    && chmod +x /app/publish/wait-for-it.sh

WORKDIR /src/Tide
COPY ["Tide.Simulator/Tide.Simulator.csproj", "Tide.Simulator/"]
COPY ["Tide.Core/Tide.Core.csproj", "Tide.Core/"]
RUN dotnet restore "Tide.Simulator/Tide.Simulator.csproj"

COPY ["Tide.Core/", "Tide.Core/"]
RUN dotnet build "Tide.Core" -c Release

COPY ["Tide.Simulator/", "Tide.Simulator/"]
RUN dotnet publish "Tide.Simulator/Tide.Simulator.csproj" -c Release -o /app/publish

FROM base AS final
COPY --from=build /app/publish .
CMD ["dotnet", "Tide.Simulator.dll"]
