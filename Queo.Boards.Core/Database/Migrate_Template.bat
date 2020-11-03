rem Nur beim ersten Mal
rem call ..\..\packages\FlyWay.queo.edition\flyway\flyway.cmd -configFile=conf\flyway.properties -url=jdbc:sqlserver://[dbserver];instanceName=[dbinstancename];databaseName=[dbname] -user=[flywayuser] -password=[flywaypassword] init

rem Aktualisieren der DB
call ..\..\packages\FlyWay.queo.edition\flyway\flyway.cmd -url=jdbc:sqlserver://[dbserver];instanceName=[dbinstancename];databaseName=[dbname] -user=[flywayuser] -password=[flywaypassword] migrate
pause