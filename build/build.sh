#!/usr/bin/env bash
set -euxo pipefail

cleanup() {
  echo "Cleaning up..."
  docker-compose \
    -p $project \
    -f ../docker-compose.tests.yml \
    -f ../docker-compose.yml down \
    --remove-orphans || true

  docker stop $(docker ps -a -q) || true
  docker rm $(docker ps -a -q) || true
  docker image rm "${project}:${GIT_SHA}" || true
}

: ${GIT_SHA?"GIT_SHA env variable is required"}
: ${VERSION?"VERSION env variable is required"}
project="todo-api"

cleanup

echo "Running the tests..."
docker-compose -f ../docker-compose.yml -f ../docker-compose.tests.yml pull
docker-compose \
  -p $project \
  -f ../docker-compose.yml \
  -f ../docker-compose.tests.yml \
  up \
  --build --force-recreate --remove-orphans \
  --exit-code-from tests \
  --abort-on-container-exit \
  tests

cleanup