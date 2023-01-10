#FROM mcr.microsoft.com/dotnet/sdk:6.0
#WORKDIR /app
#ENV Symbols_Symbol "ETHBTC"
#COPY . .
#RUN dotnet restore
#ENTRYPOINT ["dotnet", "DepthQuotesProducer/DepthQuotesProducer.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DepthQuotesProducer/DepthQuotesProducer.csproj", "DepthQuotesProducer/"]
RUN dotnet restore "DepthQuotesProducer/DepthQuotesProducer.csproj"
COPY . .
WORKDIR "/src/DepthQuotesProducer"
RUN dotnet build "DepthQuotesProducer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DepthQuotesProducer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DepthQuotesProducer.dll"]