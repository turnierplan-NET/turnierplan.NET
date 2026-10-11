#!/bin/bash

if (( $# != 1 )); then
  echo "Usage: $0 <test-name>"
  exit 1
fi

test_name="$1"
echo ""
echo "Running test: $test_name"
echo ""

docker compose up -d --wait

echo ""
echo "Applying setup script"
docker compose exec postgres psql -U postgres -d turnierplan_test -f "/test-setup/$test_name.sql" > /dev/null

# TODO: Remove this part once the DDL is integrated into the main migration script
echo "Applying schema v2 DDL"
docker compose exec postgres psql -U postgres -d turnierplan_test -c "create schema turnierplan_v2;" > /dev/null
docker compose exec postgres psql -U postgres -d turnierplan_test -f "/schema-v2-ddl.sql" > /dev/null

echo "Applying migration"
docker compose exec postgres psql -U postgres -d turnierplan_test -f "/schema-migration.sql" > /dev/null

echo "Running assertions"
output_file_name="$test_name.txt"
docker compose exec postgres psql -U postgres -d turnierplan_test -f "/assertions.sql" -o "/test-output/$output_file_name" > /dev/null

echo "Creating diff"
diff "./output/$output_file_name" "./expected/$test_name.txt"
status_code=$?

echo ""
docker compose kill
docker compose down

echo ""
echo "Exiting with code $status_code"
exit $status_code
