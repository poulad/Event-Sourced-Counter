/// <reference path="event-store.d.ts" />

fromStream(`counter:foo`)
    .when({
        $init: () => {
            return {
                value: 0
            };
        },
        "counter.incremented": (state, event) => {
            state.value += event.body.count;
        },
    })
