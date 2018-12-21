# Event Sourced Counter

I'm trying to learn [Event Sourcing]

```sh
# run an instance of Event Store database
docker run -it --name eventstore --publish 2113:2113 --publish 1113:1113 eventstore/eventstore

# run the web API
cd ./EventSourcedCounter/
dotnet run
```

```sh
# start a new counter "foo"
curl -X POST --data '' http://localhost:5000/api/counters/foo

# get value for counter "foo"
curl -X GET http://localhost:5000/api/counters/foo
```

[Event Sourcing]: https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing