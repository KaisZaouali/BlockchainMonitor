FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BlockchainMonitor.Gateway/BlockchainMonitor.Gateway.csproj", "BlockchainMonitor.Gateway/"]
RUN dotnet restore "BlockchainMonitor.Gateway/BlockchainMonitor.Gateway.csproj"
COPY . .
WORKDIR "/src/BlockchainMonitor.Gateway"
RUN dotnet build "BlockchainMonitor.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlockchainMonitor.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlockchainMonitor.Gateway.dll"] 