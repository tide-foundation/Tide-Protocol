#! /bin/bash

sw=false
for i in 1 2 3 4 5 6;
do
    echo 'Running the migrations...'
    sqlcmd -b -S ${SA_HOST:-127.0.0.1} -U sa -P ${SA_PASSWORD:-P@ssw0rd} -i migration.sql && sw=true && break
    sleep 5
done

[[ $sw == true ]] && ( echo 'The migration execution was successful.' ) || ( echo 'There was an error running the migration' && exit 1 )

echo 'Listening on port 80 for a request'
nc -l -p 80

echo 'Stopping service gracefully'
