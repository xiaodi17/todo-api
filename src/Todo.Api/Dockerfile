﻿FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /app
COPY ./*.csproj .
RUN dotnet restore

COPY ./ ./
ARG VERSION
RUN dotnet publish -c Release -o ./out/ 

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
EXPOSE 80
WORKDIR /app
COPY --from=build /app/out/ .
ENTRYPOINT ["dotnet", "Todo.Api.dll"]
