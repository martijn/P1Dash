FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["P1Dash.csproj", "./"]
RUN dotnet restore "P1Dash.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "P1Dash.csproj" --no-restore -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "P1Dash.csproj" --no-restore -c Release -o /app/publish

FROM base AS final
WORKDIR /app
VOLUME /app/Storage
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "P1Dash.dll"]
