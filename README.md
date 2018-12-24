# Event Sourced Counter

Trying out [Event Sourcing] design

## Design

![Diagram](./design-diagram.png)

## Run

```sh
# run an instance of Event Store in the background
docker run --detach --name esc-eventstore --publish 2113:2113 --publish 1113:1113 eventstore/eventstore

# run an instance of Redis in the background
docker run --detach --name esc-redis --publish 6379:6379 redis
```

```sh
# run the web API
cd ./EventSourcedCounter/
dotnet run
```

```sh
# start a new counter "foo"
curl -X POST --data '' "http://localhost:5000/api/counters/foo"

# get value for counter "foo"
curl -X GET "http://localhost:5000/api/counters/foo"

# increment it by 1 and then by 3
curl -X PATCH "http://localhost:5000/api/counters/foo"
curl -X PATCH "http://localhost:5000/api/counters/foo?count=3"

# get value for counter "foo" again
curl -X GET "http://localhost:5000/api/counters/foo"
```

```sh
# run services
docker start esc-eventstore esc-redis

# connect to Redis instance
docker run --rm -it --link esc-redis:redis redis redis-cli -h redis -p 6379
```

## Roadmap

- [X] Event Store
- [X] Read Model: Redis
- [ ] Read Model: Postgres
- [ ] Read Model: Mongo
- [ ] Read Model: Neo4j
- [ ] Angular SPA
- [ ] Angular Material
- [X] Catch-up subscriptions
- [ ] CQRS
- [ ] Snapshots
- [ ] Dockerized Services
- [ ] Travis-CI multi-staged builds
- [ ] Clustered EventStore with 3 nodes
- [ ] Deploy to Heroku
- [ ] Performance Tests
- [ ] [Cucumber] Definitions
- [ ] DDD Concepts Applied
- [ ] System Design Diagrams
- [ ] Try RX for Event Handlers?
- [ ] Try PouchDb to sync events and notifications?
- [ ] Try [Blazor] and WASM?

[Event Sourcing]: https://www.erikheemskerk.nl/event-sourcing-awesome-powerful-different/
[PuchDb]: https://github.com/pouchdb/pouchdb
[Cucumber]: http://docs.cucumber.io/
[Blazor]: https://blazor.net