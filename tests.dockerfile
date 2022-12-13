FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app/
COPY ./src/Todo.Api/*.csproj ./src/Todo.Api/
COPY ./src/Todo.Tests/*.csproj ./src/Todo.Tests/
COPY ./src/Todo.Tests/appSettings.json ./src/Todo.Tests/
COPY ./todo-api.sln ./
RUN dotnet restore

COPY ./src/ ./src/

ENTRYPOINT ["dotnet", "test"]