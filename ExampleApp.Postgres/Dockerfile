FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ExampleApp.Postgres/ExampleApp.Postgres.csproj", "ExampleApp.Postgres/"]
COPY ["WorkflowEngine/WorkflowEngine.csproj", "WorkflowEngine/"]
RUN dotnet restore "ExampleApp.Postgres/ExampleApp.Postgres.csproj"
COPY . .
WORKDIR "/src/ExampleApp.Postgres"
RUN dotnet build "./ExampleApp.Postgres.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ExampleApp.Postgres.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExampleApp.Postgres.dll"]
