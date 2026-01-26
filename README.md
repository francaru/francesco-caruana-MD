# francesco-caruana-MD
This repository contains my solution to a technical assessment provided as part of a recruitment process. It demonstrates my approach to problem-solving, code structure, and best practices.


# RabbitMQ

docker run -idt --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:latest


# Postgres

docker run -idt --rm --name postgres -e POSTGRES_USER=user -e POSTGRES_PASSWORD=pass -e POSTGRES_DB=db -p 5432:5432 postgres:latest


# Databsde Migrations

1. Open Package Mananage Console.
2. Set the default project to `OperationalService`.
3. Run `Add-Migration "<MessageWithNoSpaces>"` to generate a new revision.
4. Run `Update-Database` to apply the revision onto the connected database.


# OpenTelemetery Visualisation

docker run -idt --rm --name jaeger -p 16686:16686 -p 6831:6831/udp jaegertracing/all-in-one:1.21