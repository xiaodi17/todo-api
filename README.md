# todo-api

A containerized Web API built with .net 6, follows CI/CD using GitHub Actions

## Local Run
To run locally start up the dependent services using...
```
docker-compose up -d
```
Then run the app in your preferred IDE or using the cli...
```
dotnet run --project ./src/Todo.Api
```


## Run tests in docker

```sh
docker compose -f docker-compose.yml  -f docker-compose.tests.yml up --build --force-recreate --remove-orphans --exit-code-from tests --abort-on-container-exit tests
```

## Run tests out-of-docker

```sh
docker compose up -d --remove-orphans
```

Now, you can either use the test runner of choice to run any tests, or use it to debug tests and application under test as normal.

To clean-up after, run

```sh
docker compose down
```
