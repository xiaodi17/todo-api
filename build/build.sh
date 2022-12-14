#!/usr/bin/env bash
set -euxo pipefail
project="todo-api"

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