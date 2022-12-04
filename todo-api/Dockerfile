FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["todo-api/todo-api.csproj", "todo-api/"]
RUN dotnet restore "todo-api/todo-api.csproj"
COPY . .
WORKDIR "/src/todo-api"
RUN dotnet build "todo-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "todo-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "todo-api.dll"]
