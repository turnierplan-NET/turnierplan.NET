## turnierplan.NET &middot; SQL Scripts

This directory contains the SQL script `schema-migration.sql` to execute a migration from the v1 database schema used until 2026.3 to the v2 database schema for 2027.1 and newer releases. Instructions regarding usage are included in the script itself. Additionally, you can find corresponding tests in the `test` subdirectory. 

Each test is composed of a setup SQL script and a test expectation file. The tests are executed via a bash script:

```shell
cd test/schema-migration
chmod +x execute-test.sh
./execute-test.sh <test_name>
```

The script will run through the following steps:

1. Start a temporary PostgreSQL database using the accompanying `docker-compose.yaml` file.
2. Apply the test setup SQL script `test/schema-migration/setup/<test_name>.sql`. The setup script is a database dump created using `pg_dump` which will create a copy of the v1 schema and populate it with some specific test data.
3. Apply the migration script `schema-migration.sql`.
4. Run through the test assertions defined as `select` statements inside `test/schema-migration/assertions.sql` and print the resulting output to a file.
5. Use `diff` to compare the output from the test assertions with the expected output `test/schema-migration/setup/<test_name>.txt`.
6. Shut down and delete the temporary PostgreSQL database container.
7. If no differences are found, exit with code `0` - otherwise exit with code `1`.
